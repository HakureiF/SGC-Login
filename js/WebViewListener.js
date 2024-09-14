/***
 * 该文件下所有函数均通过接受webview消息触发
 */

// var joinRoom_antishake = true;

/**
 * 精灵放入背包，data为要放入背包的精灵id
 * @param {number[]} data 
 */
async function addToBag(data) {
    let i;
    const ids = data;

    var bagMap = PetManager.getBagMap(); //当前主背包
    for (i = 0; i < bagMap.length; i++) { //遍历主背包，将所有精灵放入仓库
        await PetManager.bagToStorage(bagMap[i].catchTime);
        console.log('toStorage', bagMap[i].id)
    }
    var secondBagMap = PetManager.getSecondBagMap(); //当前副背包
    for (i = 0; i < secondBagMap.length; i++) { //遍历副背包，将所有的精灵放入仓库
        await PetManager.secondBagToStorage(secondBagMap[i].catchTime);
        console.log('sBagtoStorage', secondBagMap[i].id)
    }
    await PetStorage2015InfoManager.getTotalInfo(); //刷新仓库内精灵数据
    var allPets = PetStorage2015InfoManager.allInfo; //仓库内精灵数据
    for (i = 0; i < ids.length; i++) { //遍历仓库，找到第一个符合id的精灵放入
        for (let j = 0; j < allPets.length; j++) {
            if (allPets[j].id === ids[i]) {
                PetStorage2015MovePetManager.movePetToBag(allPets[j]);
                console.log('toBag', allPets[j])
                await PetStorage2015InfoManager.getTotalInfo();
                break;
            }
        }
    }
}

async function addToBagFull(data) {
    let i;

    var secondBagMap = PetManager.getSecondBagMap(); //当前副背包
    for (i = 0; i < secondBagMap.length; i++) { //遍历副背包，将pick精灵精灵放入仓库
        if (data[secondBagMap[i].catchTime]) {
            await PetManager.secondBagToStorage(secondBagMap[i].catchTime);
            console.log('sBagtoStorage', secondBagMap[i].id)
        }
    }
    var bagMap = PetManager.getBagMap(); //当前主背包
    for (i = 0; i < bagMap.length; i++) { //遍历主背包，将pick精灵放入仓库，未pick精灵放进副背包
        if (data[bagMap[i].catchTime]) {
            await PetManager.bagToStorage(bagMap[i].catchTime);
            console.log('toStorage', bagMap[i].id)
        } else {
            await PetManager.bagToSecondBag(bagMap[i].catchTime);
        }
    }

    await PetStorage2015InfoManager.getTotalInfo(); //刷新仓库内精灵数据
    // data.forEach((id, pet) => {
    //     PetStorage2015MovePetManager.movePetToBag(pet)
    //     console.log('toBag', pet)
    // })s
    for (var j = 0; j < 6; j++) {
        PetStorage2015MovePetManager.movePetToBag(data[j])
        console.log('toBag', data[j])
    }
}
/**
 * 创建房间
 */
function createRoom() {
    // 打开建房界面，激活peakJihadFreeWar模块
    ModuleManager.beginShow(
        "peakJihadFreeWar",
        ["peakJihadFreeWar"],
        "PeakJihadCreateARoomPanel"
    );
    // 发送socket建房消息
    setTimeout(() => {
        SocketConnection.send(45135, 1, 1, undefined, 2);
    }, 500);
    // 打开房间界面
    setTimeout(() => {
        ModuleManager.beginShow("peakJihadFreeWar", [], "PeakJihadRoomPanel");
        SocketConnection.addCmdListener(CommandID.FIGHT_OVER, fightOver, FightOverController);
    }, 1000)
    setTimeout(() => {
        PetManager._cureAll(true, true)
    }, 1500)
}
/**
 * 加入房间
 * @param {number} data 
 */
function joinRoom(data) {
    // if (joinRoom_antishake) {
    //     joinRoom_antishake = false
    SocketConnection.sendByQueue(45136, [1, data], function (e) {
        i.roomid = "";
        ModuleManager.showModule("peakJihadFreeWar", ["peakJihadFreeWar"], null, "PeakJihadRoomPanel")
        SocketConnection.addCmdListener(CommandID.FIGHT_OVER, fightOver, FightOverController);
        setTimeout(readyFight, 1000)
        PetManager._cureAll(true, true)
        console.log("after join room")
    })
    // }
}
/**
 * 自由战准备
 */
function readyFight() {
    // joinRoom_antishake = true
    console.log("readyFight")
    PeakJihadController.getFristBagALLPetLvIsFull2() ?
        SocketConnection.sendByQueue(45136, [3, 0], function () { Alarm.show("已经通过发包准备，请勿点击准备按钮，等待房主开始对局即可") }) :
        Alarm.show("出战背包中的精灵还有未满级或者不满血的哦！点设置阵容按钮进行设置吧！")
}
/**
 * 关闭自由战房间
 */
function shutRoom() {
    // setTimeout(() => {
    //     SocketConnection.sendByQueue(45136, [5, 0], function() {
    //         ModuleManager.hideModule(peakJihadFreeWar.PeakJihadRoomPanel),
    //         ModuleManager.showModule("peakJihadFirstPage", ["peakJihadFirstPage"], null, "PeakJihadFreeMode")
    //     })
    // }, 1000)
}

function quitRoom() {
    // setTimeout(() => {
    //     SocketConnection.sendByQueue(45136, [2, 0], function() {
    //         ModuleManager.hideModule(peakJihadFreeWar.PeakJihadRoomPanel),
    //         ModuleManager.showModule("peakJihadFirstPage", ["peakJihadFirstPage"], null, "PeakJihadFreeMode")
    //     })
    // }, 500)
}
/**
 * 返回魂印id中最大的id
 * @param {*} effectList 
 * @returns 
 */
function getEffectId(effectList) {
    var tmpEffectId = 0
    for (var i=0; i<effectList.length; i++) {
        if (effectList[i].effectID > tmpEffectId) {
            tmpEffectId = effectList[i].effectID
        }
    }
    return tmpEffectId
}
/**
 * 获取背包精灵数据
 */
function getBag(webviewData) {
    var firstBagDetail = PetManager._bagMap._content;
    var secondBagDetail = PetManager._secondBagMap._content

    var ids = []
    var firstBag = []
    var secondBag = []

    var firstBagkeys = Object.keys(firstBagDetail);
    for (var i = 0; i < firstBagkeys.length; i++) {
        effectList = firstBagDetail[firstBagkeys[i]].effectList
        firstBag.push({
            id: firstBagDetail[firstBagkeys[i]].id,
            catchTime: firstBagDetail[firstBagkeys[i]].catchTime,
            level: firstBagDetail[firstBagkeys[i]].level,
            effectID: effectList && effectList.length > 0 ? getEffectId(effectList) : 0,
            marks: [firstBagDetail[firstBagkeys[i]].skillMark, firstBagDetail[firstBagkeys[i]].abilityMark, firstBagDetail[firstBagkeys[i]].commonMark],
            skillArray: firstBagDetail[firstBagkeys[i]].skillArray,
            hideSKill: firstBagDetail[firstBagkeys[i]].hideSKill,
            state: 0
        })
        ids.push(firstBagDetail[firstBagkeys[i]].id)
    }

    var secondBagkeys = Object.keys(secondBagDetail);
    for (var i = 0; i < secondBagkeys.length; i++) {
        effectList = secondBagDetail[secondBagkeys[i]].effectList
        secondBag.push({
            id: secondBagDetail[secondBagkeys[i]].id,
            catchTime: secondBagDetail[secondBagkeys[i]].catchTime,
            level: secondBagDetail[secondBagkeys[i]].level,
            effectID: effectList && effectList.length > 0 ? getEffectId(effectList) : 0,
            marks: [secondBagDetail[secondBagkeys[i]].skillMark, secondBagDetail[secondBagkeys[i]].abilityMark, secondBagDetail[secondBagkeys[i]].commonMark],
            skillArray: secondBagDetail[secondBagkeys[i]].skillArray,
            hideSKill: secondBagDetail[secondBagkeys[i]].hideSKill,
            state: 0
        })
        ids.push(secondBagDetail[secondBagkeys[i]].id)
    }
    var mess = {
        type: 'bagInfo',
        signal: webviewData.Signal,
        data: ids,
        data2: [...firstBag, ...secondBag]
    }
    console.log(mess)
    window.chrome.webview.postMessage(JSON.stringify(mess));
}
/**
 * 获取套装id
 */
function getSuit(signal) {
    var suit = SuitXMLInfo.getSuitIDs(MainManager.actorInfo.clothIDs)
    if (suit.length > 0) {
        var mess = { type: 'suitId', data: suit[0], signal: signal }
        window.chrome.webview.postMessage(JSON.stringify(mess));
    } else {
        var mess = { type: 'suitId', data: 0, signal: signal }
        window.chrome.webview.postMessage(JSON.stringify(mess));
    }
}
/**
 * 获取精英收藏数据
 */
function getElite() {
    if (PetStorage2015InfoManager.eliteInfo === undefined) {
        var mess = { type: 'eliteInfoUnLoad', data: 0 };
        console.log(mess)
        window.chrome.webview.postMessage(JSON.stringify(mess));
        return;
    }

    const elite = PetStorage2015InfoManager.eliteInfo;
    var ids = [];
    for (let i = 0; i < elite.length; i++) {
        ids.push(elite[i].id);
    }
    var mess = { type: 'eliteInfo', data: ids };
    console.log(mess)
    window.chrome.webview.postMessage(JSON.stringify(mess));
}
/**
 * 监听对战结果面板点击事件
 */
function afterFightClick() {
    EventManager.removeEventListener(PetFightEvent.ALARM_CLICK, afterFightClick)
    var mess = { type: 'fightOverClick', data: 0 };
    window.chrome.webview.postMessage(JSON.stringify(mess));
    PeakJihadController.getFristBagALLPetLvIsFull2() ? 0 : PetManager._cureAll(true, true)
}
/**
 * 战斗结束执行
 */
function fightOver(t) {
    SocketConnection.removeCmdListener(CommandID.FIGHT_OVER, fightOver, FightOverController)

    // 监听结束面板点击
    EventManager.addEventListener(PetFightEvent.ALARM_CLICK, afterFightClick)
    setTimeout(function () {
        console.log("isWinner:", FightManager.isWin)
        var mess = { type: 'isWinner', data: FightManager.isWin };
        window.chrome.webview.postMessage(JSON.stringify(mess));
    }, 1000)
    // joinRoom_antishake = true
}
/**
 * 监听战斗结果函数
 */
// function fightListen() {
//     SocketConnection.removeCmdListener(CommandID.FIGHT_OVER, fightOver, FightOverController);
//     SocketConnection.addCmdListener(CommandID.FIGHT_OVER, fightOver, FightOverController);
// }
/**
 * 获取对手米米号
 */
function getRivalMimiId() {
    KTool.getMultiValue([3314], function (t) {
        var mess = { type: 'rivalMimiId', data: t[0] };
        window.chrome.webview.postMessage(JSON.stringify(mess));
    })
}
/**
 * 获取自己米米号
 * @param {*} webviewData 
 */
function getSelfMimiId(webviewData) {
    let mimi = MainManager.actorID
    var mess = { type: 'mimiId', data: mimi, signal: webviewData.Signal };
    console.log(mess)
    window.chrome.webview.postMessage(JSON.stringify(mess));
}
/**
 * 恢复背包
 * @param {*} data 
 */
async function reStoreBag(data) {
    var bagMap = PetManager.getBagMap(); //当前主背包
    for (var i = 0; i < bagMap.length; i++) { //遍历主背包，将所有精灵放入仓库
        await PetManager.bagToStorage(bagMap[i].catchTime);
        console.log('toStorage', bagMap[i].id)
    }
    var secondBagMap = PetManager.getSecondBagMap(); //当前副背包
    for (var i = 0; i < secondBagMap.length; i++) { //遍历副背包，将所有的精灵放入仓库
        await PetManager.secondBagToStorage(secondBagMap[i].catchTime);
        console.log('sBagtoStorage', secondBagMap[i].id)
    }

    await PetStorage2015InfoManager.getTotalInfo();
    for (var j = 0; j < data.length - 6; j++) {
        PetStorage2015MovePetManager.movePetToBag(data[j])
    }
    for (var j = 0; j < data.length - 6; j++) {
        await PetManager.bagToSecondBag(data[j].catchTime);
    }
    for (var j = data.length - 6; j < data.length; j++) {
        PetStorage2015MovePetManager.movePetToBag(data[j])
    }
}
/**
 * 主处理函数
 * @param {*} event 
 */
function handle(event) {
    if (!GuideManager.isCompleted()) {
        //未登录游戏，阻止一切操作
        var mess = { type: 'unLogin', data: 0 };
        console.log(mess)
        window.chrome.webview.postMessage(JSON.stringify(mess));
        return;
    }

    console.log(event.data);
    if (event.data.Type === 'addToBag') {
        addToBag(event.data.Data)
    }
    if (event.data.Type === 'addToBagFull') {
        addToBagFull(event.data.Data)
    }
    if (event.data.Type === 'joinRoom') {
        joinRoom(event.data.Data)
    }
    if (event.data.Type === 'createRoom') {
        createRoom()
    }
    if (event.data.Type === 'shutRoom') {
        shutRoom()
    }
    if (event.data.Type === 'quitRoom') {
        quitRoom()
    }
    if (event.data.Type === 'getBag') {
        getBag(event.data)
    }
    if (event.data.Type === 'getSuit') {
        getSuit(event.data.Signal)
    }
    if (event.data.Type === 'getElite') {
        getElite()
    }
    if (event.data.Type === 'getRivalMimiId') {
        getRivalMimiId()
    }
    if (event.data.Type === 'getSelfMimiId') {
        getSelfMimiId(event.data)
    }
    if (event.data.Type === 'reStoreBag') {
        reStoreBag(event.data.Data)
    }
}
//webview消息监听器
window.chrome.webview.removeEventListener('message', handle, true);
window.chrome.webview.addEventListener('message', handle, true);
