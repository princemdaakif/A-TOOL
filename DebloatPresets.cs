namespace AAKIFTools.Data;

public static class DebloatPresets
{
    public static readonly Dictionary<string, string[]> Presets = new()
    {
        ["Samsung Bloat"] = new[]
        {
            "com.samsung.android.bixby.agent",
            "com.samsung.android.bixby.service",
            "com.samsung.android.bixbyvision.framework",
            "com.samsung.android.app.tips",
            "com.samsung.android.app.spage",
            "com.samsung.android.game.gamehome",
            "com.samsung.android.game.gos",
            "com.samsung.android.game.gametools",
            "com.samsung.android.knox.containeragent",
            "com.samsung.android.dialer",
            "com.sec.android.app.samsungapps",
            "com.samsung.android.app.galaxyfinder",
            "com.samsung.android.aware.service",
            "com.samsung.android.aremoji",
            "com.samsung.android.arzone",
            "com.samsung.android.stickercenter",
            "com.samsung.android.app.sharelive",
            "com.samsung.android.smartswitchassistant",
            "com.samsung.android.mobileservice",
            "com.samsung.android.privateshare",
            "com.samsung.android.scloud",
            "com.samsung.android.rubin.app",
            "com.samsung.android.da.daagent",
            "com.sec.android.widgetapp.samsungrecommends",
            "com.samsung.android.app.sbrowser",
            "com.samsung.android.easysetup",
            "com.samsung.android.messaging",
            "com.samsung.android.app.social",
            "com.samsung.android.tvplus",
            "com.samsung.android.video",
        },

        ["Xiaomi/MIUI Bloat"] = new[]
        {
            "com.miui.analytics",
            "com.miui.systemAdSolution",
            "com.miui.daemon",
            "com.miui.msa.global",
            "com.xiaomi.mipicks",
            "com.miui.global.packageinstaller",
            "com.miui.miservice",
            "com.miui.cleanmaster",
            "com.miui.video",
            "com.miui.yellowpage",
            "com.miui.hybrid",
            "com.miui.hybrid.accessory",
            "com.xiaomi.joyose",
            "com.xiaomi.gamecenter.sdk.service",
            "com.miui.fm",
            "cn.wps.moffice_eng",
            "com.miui.weather2",
            "com.miui.compass",
            "com.xiaomi.scanner",
            "com.miui.greenguard",
            "com.sohu.inputmethod.sogou.xiaomi",
            "com.google.android.apps.turbo",
        },

        ["OnePlus/OxygenOS Bloat"] = new[]
        {
            "com.oneplus.brickmode",
            "com.oneplus.opbackup",
            "com.oneplus.filemanager",
            "com.oneplus.gallery",
            "net.oneplus.weather",
            "com.oneplus.tips",
            "com.oneplus.logkit",
            "com.oneplus.gamespace",
            "com.heytap.market",
            "com.heytap.usercenter",
            "com.coloros.filemanager",
            "com.oppo.market",
            "com.nearme.gamecenter",
        },

        ["Google Bloat"] = new[]
        {
            "com.google.android.apps.tachyon",
            "com.google.android.videos",
            "com.google.android.music",
            "com.google.android.apps.magazines",
            "com.google.android.apps.docs",
            "com.google.android.apps.maps",
            "com.google.android.youtube",
            "com.google.android.apps.youtube.music",
            "com.google.android.gms.location.history",
            "com.google.android.feedback",
            "com.google.android.partnersetup",
            "com.google.android.apps.wellbeing",
            "com.google.android.hotspot2",
            "com.google.android.apps.subscriptions.red",
        },

        ["Facebook / Meta"] = new[]
        {
            "com.facebook.appmanager",
            "com.facebook.services",
            "com.facebook.system",
            "com.facebook.katana",
            "com.facebook.orca",
            "com.instagram.android",
        },

        ["Microsoft Apps"] = new[]
        {
            "com.microsoft.launcher.enterprise",
            "com.microsoft.skydrive",
            "com.microsoft.office.word",
            "com.microsoft.office.excel",
            "com.microsoft.office.powerpoint",
            "com.microsoft.teams",
        },
    };

    public static readonly Dictionary<string, string> Descriptions = new()
    {
        ["Samsung Bloat"]        = "Bixby, Galaxy Store, AR features, Samsung Cloud & more",
        ["Xiaomi/MIUI Bloat"]    = "MIUI ads, analytics, junk apps bundled with MIUI",
        ["OnePlus/OxygenOS Bloat"] = "Heytap, OnePlus/OPPO pre-installed bloatware",
        ["Google Bloat"]          = "Unnecessary Google apps (Maps, YouTube, Duo, etc.)",
        ["Facebook / Meta"]       = "Pre-installed Facebook & Instagram system apps",
        ["Microsoft Apps"]        = "Pre-installed Microsoft Office & Teams apps",
    };
}
