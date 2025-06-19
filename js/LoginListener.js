/***
 * 该文件下所有函数均在登录游戏后自动注册进socket监听器
 */

var rivalId = 0 //对手米米号
var rivalPetNow = 0 //对手当前出战精灵id
var rivalAliveNum = 0 //对手存活精灵数量
//获取对手首发精灵id
function rivalFirstPet(t) {
    console.log(t.data)

    //获取房主id
    var unit8_Host = new Uint8Array(t.data.buffer.slice(8, 12))
    var hostId = unit8_Host[0] * 16777216 + unit8_Host[1] * 65536 + unit8_Host[2] * 256 + unit8_Host[3]
    console.log('hostId', hostId)
    var mess = {}
    if (rivalId === hostId) {
        mess = { type: 'isRoomOwner', data: false };
    } else {
        mess = { type: 'isRoomOwner', data: true };
    }
    console.log(mess)
    window.chrome.webview.postMessage(JSON.stringify(mess));

    //一秒后获取对手首发精灵id
    setTimeout(function () {
        if (rivalId === hostId) {
            rivalAliveNum = FightUserInfo.fighterInfos.myInfo.aliveNum
        } else {
            rivalAliveNum = FightUserInfo.fighterInfos.otherInfo.aliveNum
        }
        rivalPetNow = FighterModelFactory.enemyMode._info._petID
        var mess2 = { type: 'loggerPetId', data: rivalPetNow };
        console.log(mess2)
        window.chrome.webview.postMessage(JSON.stringify(mess2));
    }, 1000)
}
//记录对手精灵
function peakLogger(t) {
    KTool.getMultiValue([3306], function (i) {
        var fightMode = i[0] //巅峰模式
        if (fightMode === 1) { // 3v3
            KTool.getMultiValue([3308], function (t) {
                rivalId = t[0]
                console.log("rival:", rivalId)
                var ids = [];
                OtherPetInfoManager.getPetInfo(rivalId, function (t) {
                    for (var i = 0; i < t.length; i++) {
                        ids.push(t[i].id)
                    }
                    var mess = { type: 'rivalPet', data: ids };
                    console.log(mess)
                    window.chrome.webview.postMessage(JSON.stringify(mess));
                }, 1) //最后的参数1代表主背包
            })
        } else if (fightMode === 2) { // 6v6
            KTool.getMultiValue([3308], function (t) {
                rivalId = t[0]
                console.log("rival:", rivalId)
                var ids = [];
                OtherPetInfoManager.getPetInfo(rivalId, function (t) {
                    for (var i = 0; i < t.length; i++) {
                        ids.push(t[i].id)
                    }
                    OtherPetInfoManager.getPetInfo(rivalId, function (t) {
                        for (var i = 0; i < t.length; i++) {
                            ids.push(t[i].id)
                        }
                        var mess = { type: 'rivalPet', data: ids };
                        console.log(mess)
                        window.chrome.webview.postMessage(JSON.stringify(mess));
                    }, 2) //最后的参数2代表副背包
                }, 1) //最后的参数1代表主背包
            })
        }
    })
}
//主动切换与死亡切换
function listenPetChange(t) {
    var unit8 = new Uint8Array(t._data.data.buffer.slice(0, 4))
    var userId = unit8[0] * 16777216 + unit8[1] * 65536 + unit8[2] * 256 + unit8[3]
    if (userId === rivalId) { //对手进行的切换
        if (FightUserInfo.fighterInfos.otherInfo.aliveNum < rivalAliveNum) { //根据精灵数量差判断是死切还是中切
            var mess = { type: 'petDefeated', data: rivalPetNow }
            console.log(mess)
            window.chrome.webview.postMessage(JSON.stringify(mess));
            rivalAliveNum = FightUserInfo.fighterInfos.otherInfo.aliveNum
        }
        unit8 = new Uint8Array(t._data.data.buffer.slice(4, 8))
        var petId = unit8[2] * 256 + unit8[3] //对方切上来的精灵
        rivalPetNow = petId
        var mess = { type: 'loggerPetId', data: petId };
        window.chrome.webview.postMessage(JSON.stringify(mess));
    }
}

/**
 * 读取字节流里的房间号
 * @param {*} t 
 */
function readRoomId(t) {
    t.data.position = 0;
    const id = t.data.readUnsignedInt();
    const mess = { type: 'roomIdCreated', data: id };
    console.log(mess)

    setTimeout(() => {
        window.chrome.webview.postMessage(JSON.stringify(mess));
    }, 3000)
}
//轮询登录状态
function pollingLogin() {
    if (!GuideManager) {
        return
    }
    console.log("isLogin:", GuideManager.isCompleted())
    if (GuideManager.isCompleted()) {

        //登录成功后注册socket指令监听器
        SocketConnection.addCmdListener(45143, peakLogger, {}) //监听对方ban我方精灵操作
        SocketConnection.addCmdListener(2407, listenPetChange, {}) //监听精灵切换操作
        SocketConnection.addCmdListener(CommandID.NOTE_START_FIGHT, rivalFirstPet, {}) //监听对战开始
        SocketConnection.addCmdListener(45135, readRoomId); //监听创建房间

        let mimi = MainManager.actorID
        var mess = { type: 'mimiId', data: mimi };
        console.log(mess)
        window.chrome.webview.postMessage(JSON.stringify(mess));
        clearInterval(loginListener)
    }
}

var loginListener = setInterval(pollingLogin, 200)