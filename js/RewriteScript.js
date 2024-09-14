Alarm = function(t) {
    function e() {
        var e = t.call(this) || this;
        return e.percentHeight = e.percentWidth = 100,
        e.skinName = AlarmuiSkin,
        e
    }
    return __extends(e, t),
    e.prototype.childrenCreated = function() {
        this.width = LevelManager.stage.stageWidth,
        this.height = LevelManager.stage.stageHeight,
        this.btn_ok.name = "btnOk",
        t.prototype.childrenCreated.call(this)
    }
    ,
    e.show = function(t, n, r, o, i) {
        void 0 === n && (n = null),
        void 0 === r && (r = null),
        void 0 === o && (o = ""),
        void 0 === i && (i = "center");
        var s = {
            msg: t,
            applyFun: n,
            container: r,
            titleStr: o,
            textAlign: i
        };
        if (!t.includes("已经放入了你的精灵背包")) {
            EventManager.dispatchEvent(new egret.Event(e.SHOW_ALARM,!1,!1,s))
        }
    }
    ,
    e.prototype.destroy = function() {}
    ,
    e.SHOW_ALARM = "SHOW_ALARM",
    e
}(eui.Component);