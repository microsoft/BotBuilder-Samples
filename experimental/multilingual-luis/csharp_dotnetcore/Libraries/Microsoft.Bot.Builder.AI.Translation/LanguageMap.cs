// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Bot.Builder.AI.Translation
{
    public class LanguageMap
    {
        /// <summary>
        /// Names for languages in all languages.
        /// </summary>
        public static readonly string[] Names =
        {
            "العربية", "البوسنية", "البلغارية", "الكتالانية",
            "الصينية المبسطة", "الصينية التقليدية", "الكرواتية", "التشيكية", "الدانماركية",
            "الهولندية", "الإنكليزية", "الإستونية", "الفنلندية", "الفرنسية",
            "الألمانية", "اليونانية", "هايتي", "العبرية", "الهندية",
            "داو الهمونغ", "المجرية", "الإندونيسية", "الإيطالية", "اليابانية",
            "اللغة السواحيلية", "الكورية", "الكلينجون", "اللاتفية", "الليتوانية",
            "الماليزية", "المالطية", "النرويجية ‏", "فارسي", "البولندية",
            "البرتغالية", "ولاية كويريتارو اوتومي", "الرومانية", "الروسية", "اللغة السيريلية الصربية",
            "الصربية اللاتينية", "السلوفاكية", "السلوفينية", "الإسبانية", "السويدية",
            "التايلندية", "التركية", "الأوكرانية", "الأوردية", "الفيتنامية",
            "الويلزية", "يوكاتيك مايا", "arapski", "bosanski", "bugarski",
            "katalonski", "pojednostavljeni kineski", "tradicionalni kineski", "hrvatski", "češki",
            "danski", "nizozemski", "engleski", "estonski", "finski",
            "francuski", "njemački", "grčki", "haićanski kreolski", "hebrejski",
            "hindi", "hmong daw", "mađarski", "indonezijski", "talijanski",
            "japanski", "kiswahili", "korejski", "klingonski", "latvijski",
            "litvanski", "malajski", "malteški", "norveški", "perzijski",
            "poljski", "portugalski", "querétaro otomi", "rumunjski", "ruski",
            "ćir", "srpski latinski", "slovački", "slovenski", "španjolski",
            "švedski", "tajlandski", "turski", "ukrajinski", "urdu",
            "vijetnamski", "velški", "jukatanskih maja", "арабски", "босняшки",
            "български", "каталонски", "опростен китайски", "традиционен китайски", "хърватски",
            "чешки", "датски", "холандски", "английски", "естонски",
            "финландски", "френски", "немски", "гръцки", "хаитянски",
            "иврит", "хинди", "хмонг daw", "унгарски", "индонезийски",
            "италиански", "японски", "корейски", "клингонската", "латвийски",
            "литовски", "малайски", "малтийски", "норвежки", "персийски",
            "полски", "португалски", "керетаро otomi", "румънски", "руски",
            "сръбската кирилица", "сръбската латинска", "словашки", "словенски", "испански",
            "шведски", "тайландски", "турски", "украински", "урду",
            "виетнамски", "уелски", "yucatec мая", "àrab", "bosnià",
            "búlgar", "català", "xinès simplificat", "xinès tradicional", "croat",
            "txeca", "danès", "neerlandès", "anglès", "estonià",
            "finlandesa", "francès", "alemany", "grec", "crioll haitià",
            "hebreu", "hindi", "ètnia hmong que daw", "hongarès", "indonèsia",
            "italià", "japonès", "coreà", "klingon", "letònia",
            "lituà", "malai", "maltès", "noruec", "persa",
            "polonès", "portuguès", "querétaro otomí", "romanès", "rus",
            "serbi", "serbi llatí", "eslovac", "eslovè", "castellà",
            "suec", "tailandès", "turc", "ucraïnès", "urdú",
            "vietnamita", "gal·lès", "del segle xvi maya", "阿拉伯语", "波斯尼亚语",
            "保加利亚语", "加泰隆语", "简体中文", "繁体中文", "克罗地亚语",
            "捷克语", "丹麦语", "荷兰语", "英语", "爱沙尼亚语",
            "芬兰语", "法语", "德语", "希腊语", "海地克里奥尔语",
            "希伯来语", "印地语", "苗族人昂山", "匈牙利语", "印度尼西亚语",
            "意大利语", "日语", "斯瓦希里语", "朝鲜语", "克林贡语",
            "拉脱维亚语", "立陶宛语", "马来语", "马耳他语", "挪威语",
            "波斯语", "波兰语", "葡萄牙语", "克雷塔罗乙巳", "罗马尼亚",
            "俄语", "塞尔维亚西里尔文", "塞尔维亚语-拉丁文", "斯洛伐克语", "斯洛文尼亚语",
            "西班牙语", "瑞典语", "泰语", "土耳其语", "乌克兰语",
            "乌都语", "越南语", "威尔士语", "尤卡坦玛雅", "阿拉伯文",
            "波士尼亞文", "保加利亞文", "加泰隆語", "簡體中文", "繁體中文",
            "克羅地亞語", "捷克文", "丹麥文", "荷蘭文", "英語",
            "愛沙尼亞文", "芬蘭文", "法語", "德語", "希臘文",
            "海地克裡奧爾語", "希伯來文", "印度文", "匈牙利文", "印尼語",
            "義大利文", "日語", "史瓦希里文", "朝鮮語", "克林貢語",
            "拉脫維亞文", "立陶宛文", "馬來文", "馬爾他文", "挪威語",
            "波斯文", "波蘭文", "葡萄牙文", "克雷塔羅乙巳", "羅馬尼亞",
            "俄語", "塞爾維亞西瑞爾文", "塞爾維亞文-拉丁文", "斯洛伐克文", "斯洛維尼亞文",
            "西班牙文", "瑞典文", "泰語", "土耳其文", "烏克蘭文",
            "烏都語", "越南文", "威爾斯文", "尤卡坦瑪雅", "srpska ćirilica",
            "srpski latinica", "jukatanskih maja", "arabština", "bosenština", "bulharština",
            "katalánština", "zjednodušená čínština", "tradiční čínština", "chorvatština", "čeština",
            "dánština", "holandština", "angličtina", "estonština", "finština",
            "francouzština", "němčina", "řečtina", "haitská kreolština", "hebrejština",
            "hindština", "maďarština", "indonéština", "italština", "japonština",
            "korejština", "klingonská", "lotyština", "litevština", "malajština",
            "maltština", "norština", "perština", "polština", "portugalština",
            "rumunština", "ruština", "srbské cyrilici", "srbský (latinka)", "slovenština",
            "slovinština", "španělština", "švédština", "thajština", "turečtina",
            "ukrajinština", "urdština", "vietnamština", "velština", "yucatec maya",
            "arabisk", "bosnisk", "bulgarsk", "catalansk", "kinesisk, forenklet",
            "kinesisk, traditionelt", "kroatisk", "tjekkisk", "dansk", "nederlandsk",
            "engelsk", "estisk", "finsk", "fransk", "tysk",
            "græsk", "haitiansk", "hebraisk", "ungarsk", "indonesisk",
            "italiensk", "japansk", "koreansk", "lettisk", "litauisk",
            "malaysisk", "maltesisk", "norsk", "persisk", "polsk",
            "portugisisk", "rumænsk", "russisk", "serbisk kyrillisk", "serbisk latin",
            "slovakisk", "slovensk", "spansk", "svensk", "thai",
            "tyrkisk", "ukrainsk", "vietnamesisk", "walisisk", "arabisch",
            "bosnisch", "bulgaars", "catalaans", "vereenvoudigd chinees", "traditioneel chinees",
            "kroatisch", "tsjechisch", "deens", "nederlands", "engels",
            "estisch", "fins", "frans", "duits", "grieks",
            "haïtiaans", "hebreeuws", "hongaars", "indonesisch", "italiaans",
            "japans", "koreaans", "lets", "litouws", "maleis",
            "maltees", "noors", "perzisch", "pools", "portugees",
            "roemeens", "russisch", "servisch", "servisch-latijn", "slowaaks",
            "sloveens", "spaans", "zweeds", "thais", "turks",
            "oekraïens", "urdu", "vietnamees", "welsh", "yucateeks maya",
            "arabic", "bosnian", "bulgarian", "catalan", "chinese simplified",
            "chinese traditional", "croatian", "czech", "danish", "dutch",
            "english", "estonian", "finnish", "french", "german",
            "greek", "haitian creole", "hebrew", "hungarian", "indonesian",
            "italian", "japanese", "korean", "latvian", "lithuanian",
            "malay", "maltese", "norwegian", "persian", "polish",
            "portuguese", "romanian", "russian", "serbian cyrillic", "serbian latin",
            "slovak", "slovenian", "spanish", "swedish", "thai",
            "turkish", "ukrainian", "vietnamese", "araabia", "bosnia",
            "bulgaaria", "katalaani", "lihtsustatud hiina", "traditsiooniline hiina", "horvaadi",
            "tšehhi", "taani", "hollandi", "inglise", "eesti",
            "soome", "prantsuse", "saksa", "kreeka", "haiiti kreooli",
            "heebrea", "hmongi daw", "ungari", "indoneesia", "itaalia",
            "jaapani", "suahiili", "korea", "klingoni", "läti",
            "leedu", "malta", "norra", "pärsia", "poola",
            "portugali", "rumeenia", "vene", "serbia keel kirillitsas", "serbia keel ladina tähestikus",
            "slovaki", "sloveeni", "hispaania", "rootsi", "tai",
            "türgi", "ukraina", "vietnami", "kõmri", "arabia",
            "bosnia", "bulgaria", "katalaani", "yksinkertaistettu kiina", "perinteinen kiina",
            "kroaatti", "tšekki", "tanska", "hollanti", "englanti",
            "viro", "suomi", "ranska", "saksa", "kreikka",
            "haitin kieli", "heprea", "unkari", "indonesia", "italia",
            "japani", "korea", "latvia", "liettua", "malaiji",
            "malta", "norja", "persia", "puola", "portugali",
            "romania", "venäjä", "serbian kyrilliset", "serbialainen latinalainen", "slovakki",
            "sloveeni", "espanja", "ruotsi", "turkki", "ukraina",
            "vietnam", "kymri (wales)", "suomen maya", "arabe", "bosniaque",
            "bulgare", "chinois simplifié", "chinois traditionnel", "croate", "tchèque",
            "danois", "néerlandais", "anglais", "estonien", "finnois",
            "français", "allemand", "haïtien", "hébreu", "hongrois",
            "indonésien", "italien", "japonais", "coréen", "letton",
            "lituanien", "malaisien", "maltais", "norvégien", "perse",
            "polonais", "portugais", "roumain", "russe", "en serbe cyrillique",
            "serbe latin", "slovaque", "slovène", "espagnol", "suédois",
            "thaï", "ukrainien", "vietnamien", "gallois", "maya yucatèque",
            "bulgarisch", "katalanisch", "chinesisch (vereinfacht)", "chinesisch (traditionell)", "tschechisch",
            "dänisch", "holländisch", "englisch", "estnisch", "finnisch",
            "französisch", "deutsch", "griechisch", "haitianisch", "hebräisch",
            "ungarisch", "italienisch", "japanisch", "koreanisch", "klingonische",
            "lettisch", "litauisch", "malaiisch", "maltesisch", "norwegisch",
            "persisch", "polnisch", "portugiesisch", "querétaro otomí", "rumänisch",
            "serbisch-kyrillisch", "serbischer latein", "slowakisch", "slowenisch", "spanisch",
            "schwedisch", "thailändisch", "türkisch", "ukrainisch", "vietnamesisch",
            "walisisch", "yukatekisches maya", "αραβικά", "βοσνιακά", "βουλγαρικά",
            "καταλανικά", "απλοποιημένα κινεζικά", "παραδοσιακά κινεζικά", "κροατικά", "τσεχικά",
            "δανικά", "ολλανδικά", "αγγλικά", "εσθονικά", "φινλανδικά",
            "γαλλικά", "γερμανικά", "ελληνικά", "γλώσσα αϊτής", "εβραϊκά",
            "χίντι", "ουγγρικά", "ινδονησιακά", "ιταλικά", "ιαπωνικά",
            "κισουαχίλι", "κορεατικά", "κλίνγκον", "λετονικά", "λιθουανικά",
            "μαλαϊκά", "μαλτεζικά", "νορβηγικά", "περσικά", "πολωνικά",
            "πορτογαλικά", "ρουμανικά", "ρωσικά", "σερβικά κυριλλικά", "σέρβικα-λατινικά",
            "σλοβακικά", "σλοβενικά", "ισπανικά", "σουηδικά", "ταϊλανδικά",
            "τουρκικά", "ουκρανικά", "ουρντού", "βιετναμικά", "ουαλλικά",
            "μάγια του γιουκατάν", "arabik", "bosnyen", "bilgari", "chinwa senplifye",
            "chinwa tradisyonèl", "kwoasyen", "tchekoslovaki", "danwa", "olandè",
            "angle", "estoni", "fenlandè", "fwansè", "alman",
            "grèk", "kreyòl ayisyen", "ebre", "lang peyi lend", "montayn hmong daw",
            "ongaryen", "endonezi", "italyen", "japonè", "koreyen",
            "lettonie", "malais", "malte", "nòvejyen", "pèsyann",
            "lapolòy", "pòtigè", "romanyen", "wis", "lettre sèb",
            "latin sèb", "sloveni", "panyòl", "syedwa", "pon zanmitay tayilandè",
            "tik", "ikrenyen", "oudou", "vyètnamyen", "ערבית",
            "בוסנית", "בולגרית", "קטלונית", "סינית פשוטה", "סינית מסורתית",
            "קרואטית", "צ\'כית", "דנית", "הולנדית", "אנגלית",
            "אסטונית", "פינית", "צרפתית", "גרמנית", "יוונית",
            "האיטית", "עברית", "הינדי", "המונג דאו", "הונגרית",
            "אינדונזית", "איטלקית", "יפנית", "בסווהילי", "קוריאנית",
            "קלינגונית", "לטבית", "ליטאית", "מלאית", "מלטית",
            "נורווגית", "פרסית", "פולנית", "פורטוגזית", "רומנית",
            "רוסית", "סרבית קירילית", "סרבית לטינית", "סלובקית", "סלובנית",
            "ספרדית", "שוודית", "תאילנדית", "טורקית", "אוקראינית",
            "אורדו", "וייטנאמית", "וולשית", "מאיה yucatec", "अरबी",
            "बोस्नियाई", "बल्गारिया", "कातलान", "चीनी सरलीकृत", "चीनी पारंपरिक",
            "क्रोएशियाई", "चेक", "डैनिश", "डच", "अंग्रेज़ी",
            "एस्टोनियाई", "फ़िनिश", "फ़्रेंच", "जर्मन", "यूनानी",
            "हाईटियन क्रियोल", "हिब्रू", "हिंदी", "हमोंग काला कौवा", "हंगेरियन",
            "इंडोनेशियाई", "इटैलियन", "जापानी", "किस्वाहिली", "कोरियाई",
            "क्लिंगन", "लातवियाई", "लिथुआनियाई", "मलय", "माल्टीज़",
            "नॉर्वेजियन", "फ़ारसी", "पोलिश", "पुर्तगाली", "रोमानियाई",
            "रूसी", "सर्बियन सिरिलिक", "सर्बियन लैटिन", "स्लोवाक", "स्लोवेनियाई",
            "स्पैनिश", "स्वीडिश", "थाई", "तुर्की", "यूक्रेनियाई",
            "उर्दू", "वियतनामी", "वेल्श", "युकाटेक माया", "suav yooj yim zog",
            "nws yog tshuaj hmoob suav", "lus askiv", "fabkis", "lus", "hmoob daw",
            "lavxias teb sab", "mev", "thaib", "nyab laj", "arab",
            "boszniai", "bolgár", "katalán", "kínai (egyszerűsített)", "kínai (hagyományos)",
            "horvát", "cseh", "dán", "holland", "angol",
            "észt", "finn", "francia", "német", "görög",
            "haiti kreol", "héber", "magyar", "indonéz", "olasz",
            "japán", "koreai", "lett", "litván", "maláj",
            "máltai", "norvég", "perzsa", "lengyel", "portugál",
            "román", "orosz", "szerb", "szerb latin", "szlovák",
            "szlovén", "spanyol", "svéd", "török", "ukrán",
            "vietnami", "walesi", "török", "arab", "bulgaria",
            "cina sederhana", "cina tradisional", "kroasia", "ceko", "denmark",
            "belanda", "inggris", "estonia", "finlandia", "prancis",
            "jerman", "yunani", "bahasa kreol haiti", "ibrani", "hongaria",
            "indonesia", "italia", "jepang", "latvia", "lithuania",
            "melayu", "norwegia", "farsi", "polandia", "portugis",
            "rumania", "rusia", "bahasa sirilik", "serbia latin", "slovakia",
            "slovenia", "spanyol", "swedia", "turki", "vietnam",
            "arabo", "bosniaco", "bulgaro", "catalano", "cinese semplificato",
            "cinese tradizionale", "croato", "ceco", "danese", "olandese",
            "inglese", "estone", "finlandese", "francese", "tedesco",
            "greco", "haitiano", "ebraico", "ungherese", "indonesiano",
            "italiano", "giapponese", "coreano", "lettone", "lituano",
            "malese", "norvegese", "polacco", "portoghese", "rumeno",
            "russo", "alfabeto cirillico serbo", "serbo latino", "slovacco", "sloveno",
            "spagnolo", "svedese", "tailandese", "turco", "ucraino",
            "gallese", "maya yucateco", "アラビア語", "ボスニア語", "ブルガリア語",
            "カタロニア語", "簡体字中国語", "繁体字中国語", "クロアチア語", "チェコ語",
            "デンマーク語", "オランダ語", "エストニア語", "フィンランド語", "フランス語",
            "ドイツ語", "ギリシャ語", "ハイチ語", "ヘブライ語", "ヒンディー語",
            "もん族 daw", "ハンガリー語", "インドネシア語", "イタリア語", "日本語",
            "スワヒリ語", "韓国語", "クリンゴン語", "ラトビア語", "リトアニア語",
            "マレー語", "マルタ語", "ノルウェー語", "ペルシャ語", "ポーランド語",
            "ポルトガル語", "ケレタロ大富", "ルーマニア語", "ロシア語", "セルビア語キリル文字",
            "セルビア語ラテン語", "スロバキア語", "スロベニア語", "スペイン語", "スウェーデン語",
            "タイ語", "トルコ語", "ウクライナ語", "ウルドゥ語", "ベトナム語",
            "ウェールズ語", "ユカテク語マヤ", "kiarabu", "kibosnia", "kibulgaria",
            "kikatalan", "kichina rahisi", "kichina cha zamani", "kikroashia", "kicheki",
            "kidanishi", "kiholanzi", "kiingereza", "kiestonia", "kifinishi",
            "kifaransa", "kijerumani", "kigiriki", "kikrioli cha haiti", "kiebrania",
            "kihindi", "kihangaria", "kiindoneshia", "kiitaliano", "kijapani",
            "kikorea", "kilatvia", "kilithuania", "kimalay", "kimalta",
            "kinowiji", "kipersia", "kipolishi", "kireno", "kirumi",
            "kirusi", "kisirili kiserbia", "kilatini kiserbia", "kislovaki", "kislovenia",
            "kihispania", "kiswidi", "kithai", "kituruki", "kiukreini",
            "kiurdu", "kivietinamu", "kiwelshi", "아랍어", "보스니아어",
            "불가리어", "카탈로니아어", "중국어 간체", "중국어 번체", "크로아티아어",
            "체코어", "덴마크어", "네덜란드어", "영어", "에스토니아어",
            "핀란드어", "프랑스어", "독일어", "그리스어", "아이티어",
            "히브리어", "힌디어", "흐몽 어 갈가 마 귀", "헝가리어", "인도네시아어",
            "이탈리아어", "일본어", "한국어", "클 링 온", "라트비아어",
            "리투아니아어", "말레이어", "몰타어", "노르웨이어", "페르시아어",
            "폴란드어", "포르투갈어", "케레타로 otomi", "루마니아어", "러시아어",
            "세르비아어 키릴 자모", "세르비아어-라틴", "슬로바키아어", "슬로베니아어", "스페인어",
            "스웨덴어", "태국어", "터키어", "우크라이나어", "우르두어",
            "베트남어", "웨일스어", "arabic", "bosnian", "bulgarian",
            "catalan", "chinese simplified", "chinese potlhmeydaq", "croatian", "czech",
            "maghbogh", "dutch", "english hol", "estonian", "finnish",
            "french", "german", "greek", "haitian creole", "hebrew",
            "hmong daw", "hungarian", "indonesian", "italian", "japanese",
            "kiswahili", "korean", "tlhingan", "latvian", "lithuanian",
            "malay", "maltese", "norwegian", "persian", "polish",
            "portuguese", "querétaro otomi", "romanian", "russian", "serbian cyrillic",
            "serbian latin", "slovak", "slovenian", "spanish", "swedish",
            "turkish", "ukrainian", "vietnamese", "welsh", "yucatec maya",
            "arābu", "bosniešu", "bulgāru", "kataloniešu", "ķīniešu vienkāršotā",
            "ķīniešu tradicionālā", "horvātu", "čehu", "dāņu", "holandiešu",
            "angļu", "igauņu", "somu", "franču", "vācu",
            "grieķu", "haiti kreoliete", "ivrits", "ungāru", "indonēziešu",
            "itāliešu", "japāņu", "korejiešu", "latviešu", "lietuviešu",
            "malajiešu", "maltiešu", "norvēģu", "persiešu", "poļu",
            "portugāļu", "rumāņu", "krievu", "serbu kiriliskā", "serbu latīņu",
            "slovāku", "slovēņu", "spāņu", "zviedru", "taju",
            "turku", "ukraiņu", "vjetnamiešu", "velsiešu", "arabų",
            "bosnių", "bulgarų", "katalonų", "kinų supaprastinta", "kinų tradicinė",
            "kroatų", "čekų", "danų", "olandų", "anglų",
            "estų", "suomių", "prancūzų", "vokiečių", "graikų",
            "haičio kreolų", "hebrajų", "vengrų", "indoneziečių", "italų",
            "japonų", "principai", "korėjos", "latvių", "lietuvių",
            "malajų", "maltos", "norvegų", "persų", "lenkų",
            "portugalų", "rumunų", "rusų", "serbų kirilica", "serbų (lotyniška)",
            "slovakų", "slovėnų", "ispanų", "švedų", "tajų",
            "turkų", "ukrainiečių", "vietnamiečių", "valų", "jukatekų majų",
            "catalonia", "cina ringkas", "croatia", "inggeris", "finland",
            "perancis", "kreol haiti", "daw bahasa kantonis", "hungary", "itali",
            "jepun", "norway", "parsi", "poland", "romania",
            "cyrillic serbia", "latin serbia", "sepanyol", "sweden", "ukraine",
            "wales", "għarbi", "bożnijan", "bulgaru", "katalan",
            "ċiniż issimplifikat", "ċiniż tradizzjonali", "kroat", "ċeka", "daniż",
            "olandiż", "ingliż", "estonjan", "fillandiż", "franċiż",
            "ġermaniż", "grieg", "ħajtjan (krejol)", "lhudi", "ħindi",
            "ungeriż", "indoneżjan", "taljan", "ġappuniż", "korejan",
            "latvjan", "litwan", "mależjan", "malti", "norveġjan",
            "persjan", "pollak", "portugiż", "rumen", "russu",
            "cyrillic serbi", "latina serbi", "slovakk", "sloven", "spanjol",
            "svediż", "tajlandiż", "tork", "ukren", "vjetnamiż",
            "arabisk", "bosniske", "bulgarsk", "katalansk", "forenklet kinesisk",
            "tradisjonell kinesisk", "kroatisk", "tsjekkisk", "dansk", "nederlandsk",
            "engelsk", "estisk", "finsk", "fransk", "tysk",
            "gresk", "haitisk kreolsk", "hebraisk", "ungarsk", "indonesisk",
            "italiensk", "japansk", "koreansk", "latvisk", "litauisk",
            "malayisk", "maltesisk", "norsk", "polsk", "portugisisk",
            "querétaro enten", "rumensk", "russisk", "serbisk", "slovakisk",
            "slovensk", "spansk", "svenske", "tyrkisk", "ukrainsk",
            "vietnamesisk", "walisisk", "عربی", "بوسنیایی", "بلغاری",
            "کاتالان", "چینی ساده شده", "چینی سنتی", "کرواتی", "چک",
            "دانمارک", "هلندی", "انگلیسی", "استونیایی", "فنلاندی",
            "فرانسه", "آلمانی", "یونانی", "زبان کریول آییسینی", "عبری",
            "هندی", "همونگ ادم کند و تنبل", "مجارستان", "اندونزی", "ایتالیایی",
            "ژاپنی", "kiswahili تهیه شده است", "کره ای", "کلینگون", "لتونی",
            "لیتوانی", "مالایی", "مالتی", "نروژی", "فارسی",
            "لهستانی", "پرتغالی", "رومانیایی", "روسی", "صربستان سیریلیک",
            "صربستان لاتین", "اسلواکی", "اسلوونیایی", "اسپانیایی", "سوئد",
            "تایلندی", "ترکیه", "اوکراینی", "اردو", "ویتنامی",
            "ویلز", "مایا yucatec", "arabski", "bośniacki", "bułgarski",
            "kataloński", "chiński uproszczony", "chiński tradycyjny", "chorwacki", "czeski",
            "duński", "holenderski", "angielski", "estoński", "fiński",
            "francuski", "niemiecki", "grecki", "haitański", "hebrajski",
            "węgierski", "indonezyjski", "włoski", "japoński", "koreański",
            "klingoński", "łotewski", "litewski", "malajski", "maltański",
            "norweski", "perski", "polski", "portugalski", "rumuński",
            "rosyjski", "serb.", "serbski łacina", "słowacki", "słoweński",
            "hiszpański", "szwedzki", "tajski", "turecki", "ukraiński",
            "wietnamski", "walijski", "árabe", "bósnio", "búlgaro",
            "catalão", "chinês simplificado", "chinês tradicional", "croata", "tcheco",
            "dinamarquês", "holandês", "inglês", "estoniano", "finlandês",
            "francês", "alemão", "grego", "hebraico", "húngaro",
            "indonésio", "japonês", "letão", "malaio", "maltês",
            "norueguês", "polonês", "português", "romeno", "sérvio cirílico",
            "sérvio-latino", "eslovaco", "esloveno", "espanhol", "sueco",
            "tailandês", "ucraniano", "galês", "iucateque", "bosnio",
            "catalán", "txino simplificado", "txino pa mahä'mu̲", "checo", "danés",
            "holandés", "ra ingles", "estonio", "finlandés", "francés",
            "alemän", "griego", "criollo haitiano", "hebreo", "indonesio",
            "japonés", "letón", "malayo", "maltés", "noruego",
            "polaco", "portugués", "maxei ñäñho", "rumano", "ruso",
            "cirílico servio", "serbio latino", "hñämfo̲", "tailandés", "galés",
            "maya yucateco", "arabă", "bosniacă", "bulgară", "catalană",
            "chineză simplificată", "chineză tradiţională", "croată", "cehă", "daneză",
            "olandeză", "engleză", "estonă", "finlandeză", "franceză",
            "germană", "greacă", "creolă haitiană", "ebraică", "maghiară",
            "indoneziană", "italiană", "japoneză", "coreeană", "klingoniană",
            "letonă", "lituaniană", "malaieză", "malteză", "norvegiană",
            "persană", "poloneză", "portugheză", "română", "rusă",
            "sârbă-chirilică", "sârbă-latină", "slovacă", "slovenă", "spaniolă",
            "suedeză", "turcă", "ucraineană", "vietnameză", "velşă",
            "арабский", "боснийский", "болгарский", "каталанский", "китайский упрощенный",
            "китайский традиционный", "хорватский", "чешский", "датский", "нидерландский",
            "английский", "эстонский", "финский", "французский", "немецкий",
            "греческий", "гаитянский", "иврит", "хинди", "оупж хмонг",
            "венгерский", "индонезийский", "итальянский", "японский", "суахили",
            "корейский", "клингонский", "латышский", "литовский", "малайский",
            "мальтийский", "норвежский", "персидский", "польский", "португальский",
            "керетаро отоми", "румынский", "русский", "сербская кириллица", "сербская латинской",
            "словацкий", "словенский", "испанский", "шведский", "тайский",
            "турецкий", "украинский", "урду", "вьетнамский", "валлийский",
            "юкатекский", "арапски", "босански", "бугарски", "каталонски",
            "кинески поједностављени", "кинески традиционални", "хрватски", "чешки", "дански",
            "холандски", "енглески", "естонски", "фински", "француски",
            "немачки", "грчки", "хаићански креолски", "хебрејски", "хинду",
            "госпоро хмонга", "мађарски", "индонежански", "италијански", "јапански",
            "свахили", "кореански", "клингонски", "летонски", "литвански",
            "малајски", "малтешки", "норвешки", "персијски", "пољски",
            "португалски", "qуерéтаро отоми", "румунски", "руски", "српска ћирилица",
            "српски латиница", "словачки", "словенски", "шпански", "шведски",
            "тајландски", "турски", "украјински", "вијетнамски", "велшански",
            "yуцатец маyа", "kineski pojednostavljeni", "kineski tradicionalni", "holandski", "nemački",
            "hindu", "gospoрo hmonga", "indonežanski", "italijanski", "svahili",
            "koreanski", "letonski", "persijski", "rumunski", "španski",
            "velšanski", "arabčina", "bosniansky jazyk", "bulharčina", "katalánčina",
            "čínština (zjednodušená)", "čínština (tradičná)", "chorvátčina", "čeština", "dánčina",
            "holandčina", "angličtina", "estónčina", "fínčina", "francúzština",
            "nemčina", "gréčtina", "haitskej kreolský", "hebrejčina", "hindčina",
            "maďarčina", "indonézština", "taliančina", "japončina", "kórejčina",
            "lotyština", "litovčina", "malajčina", "maltčina", "nórčina",
            "perzština", "poľština", "portugalčina", "otomo querétaro", "rumunčina",
            "ruština", "srbský (cyrilika)", "srbská latinčina", "slovenčina", "slovinčina",
            "španielčina", "švédčina", "thajčina", "turečtina", "ukrajinčina",
            "urdčina", "vietnamčina", "waleština", "arabščina", "bošnjaški jezik",
            "bolgarščina", "katalonščina", "poenostavljena kitajščina", "tradicionalna kitajščina", "hrvaščina",
            "češčina", "danščina", "nizozemščina", "angleščina", "estonščina",
            "finščina", "francoščina", "nemščina", "grščina", "hebrejščina",
            "hindijščina", "madžarščina", "indonezijščina", "italijanščina", "japonščina",
            "korejščina", "klingonščina", "latvijščina", "litovščina", "malajščina",
            "malteščina", "norveščina", "perzijščina", "poljščina", "portugalščina",
            "romunščina", "ruščina", "srbsko", "srb latinski", "slovaščina",
            "slovenščina", "španščina", "švedščina", "tajščina", "turščina",
            "ukrajinščina", "vietnamščina", "valižanščina", "chino simplificado", "chino tradicional",
            "inglés", "alemán", "español", "arabiska", "bosniska",
            "bulgariska", "katalanska", "kinesiska, förenklad", "kinesiska, traditionell", "kroatiska",
            "tjeckiska", "danska", "nederländska", "engelska", "estniska",
            "finska", "franska", "tyska", "grekiska", "haitiska",
            "hebreiska", "ungerska", "indonesiska", "italienska", "japanska",
            "koreanska", "klingonska", "lettiska", "litauiska", "malajiska",
            "maltesiska", "norska", "persiska", "polska", "portugisiska",
            "rumänska", "ryska", "serbisk kyrilliska", "serbiska-latin", "slovakiska",
            "slovenska", "spanska", "svenska", "turkiska", "ukrainska",
            "vietnamesiska", "walesiska", "อาหรับ", "บอสเนีย", "เบลเยียม",
            "คาตาลัน", "จีนประยุกต์", "จีนดั้งเดิม", "โครเอเชีย", "เชก",
            "เดนมาร์ก", "ดัทช์", "อังกฤษ", "เอสโทเนีย", "ฟินแลนด์",
            "ฝรั่งเศส", "เยอรมัน", "กรีก", "ชาวเฮติ", "เฮบรู",
            "ฮินดี", "ม้งทะเล", "ฮังการี", "อินโดนีเซีย", "อิตาลี",
            "ญี่ปุ่น", "กีสวาฮีลี", "เกาหลี", "คลิงออน", "ลัตเวีย",
            "ลิธัวเนีย", "มาเลย์", "มอลเทส", "นอร์เวย์", "เปอร์เซีย",
            "โปแลนด์", "โปรตุเกส", "otomi ซึ่ง", "โรมาเนีย", "รัสเซีย",
            "เซอร์เบียซิริลลิก", "ละตินเซอร์เบีย", "สโลวัค", "สโลวีเนีย", "สเปน",
            "สวีเดน", "ไทย", "ตุรกี", "ยูเครน", "เออร์ดู",
            "เวียดนาม", "เวลช์", "arapça", "boşnakça", "bulgarca",
            "katalanca", "basitleştirilmiş çince", "geleneksel çince", "hırvatça", "çekçe",
            "danca", "hollanda dili", "İngilizce", "estonca", "fince",
            "fransızca", "almanca", "yunanca", "haitice", "İbranice",
            "hintçe", "macarca", "endonezya dili", "İtalyanca", "japonca",
            "kore dili", "letonca", "litvanca", "malay dili", "malta dili",
            "norveç dili", "farsça", "lehçe", "portekizce", "rumence",
            "rusça", "sırpça", "sırp latin", "slovakça", "slovence",
            "İspanyolca", "İsveç dili", "tay dili", "türkçe", "ukrayna dili",
            "urduca", "vietnam dili", "galce", "арабська", "боснійська",
            "болгарська", "каталанська", "китайська (спрощене письмо)", "китайська (традиційне письмо)", "хорватська",
            "чеська", "датська", "нідерландська", "англійська", "естонська",
            "фінська", "французька", "німецька", "грецька", "гаїтянська креольська",
            "іврит", "хінді", "хмонг жи", "угорська", "індонезійська",
            "італійська", "японська", "суахілі", "корейська", "клінгонскій",
            "латиська", "литовська", "малайська", "мальтійська", "норвезька",
            "перська", "польська", "португальська", "керетаро отом", "румунська",
            "російська", "сербська кирилична", "сербська (латиниця)", "словацька", "словенська",
            "іспанська", "шведська", "тайська", "турецька", "українська",
            "в\'єтнамська", "валлійська", "юкатецька", "بوسنين", "بلگيرين",
            "کٹالن", "آسان چائنيز", "ثقافتی چائنيز", "کروٹيئن", "کزکھ",
            "ڈينش", "ڈچ", "انگريزی", "اسٹونين", "فنش",
            "فرنچ", "جرمن", "يونانی", "ہيتيئن کريول", "اسرائيلی",
            "ہندی", "ہنگرين", "انڈونيشين", "اطالوی", "جاپانی",
            "سواحلی", "کوريئن", "کلنگاون", "لاتوين", "لتھونيئن",
            "مالے", "مالٹيز", "نارويجين", "پولش", "پرتگال",
            "querétaro اوٹوما", "رومانيئن", "رشئن", "سربیائی سیریلیائی", "سربیائی لاطینی",
            "سلووک", "سلووينيئن", "سپينش", "سؤڈش", "تھائ",
            "ترکش", "يوکرينيئن", "ويتناميز", "ويلش", "یوکٹیک مایا",
            "tiếng ả rập", "tiếng bulgaria", "tiếng catalan", "tiếng trung giản thể", "trung quốc truyền thống",
            "séc", "đan mạch", "hà lan", "tiếng anh", "tiếng estonia",
            "phần lan", "tiếng pháp", "đức", "hy lạp", "tiếng hebrew",
            "tiếng hin-ddi", "ý", "nhật bản", "hàn quốc", "tiếng latvia",
            "tiếng mã lai", "xứ man-tơ", "na uy", "ba tư", "ba lan",
            "tiếng bồ đào nha", "rumani", "nga", "serbia cyrillic", "tiếng slovak",
            "tiếng slovenia", "tiếng tây ban nha", "thụy điển", "thái lan", "thổ nhĩ kỳ",
            "tiếng ukraina", "tiếng urdu", "việt nam", "tiếng wales", "arabeg",
            "bosnieg", "bwlgareg", "catalaneg", "tsieinëeg (wedi ei symyleiddio)", "tsieinëeg (traddodiadol)",
            "croateg", "tsieceg", "daneg", "iseldireg", "saesneg",
            "estoneg", "ffineg", "ffrangeg", "almaeneg", "iaith groeg",
            "creol haiti", "hebraeg", "hwngareg", "indoneseg", "eidaleg",
            "siapaneeg", "corëeg", "latfeg", "lithwaneg", "maleieg",
            "malteg", "norwyeeg", "perseg", "pwyleg", "portiwgeeg",
            "rwmaneg", "rwsieg", "cyrilig serbia", "lladin serbia", "slofaceg",
            "slofeneg", "sbaeneg", "swedeg", "twrceg", "wrceineg",
            "wrdw", "fietnameg", "cymraeg", "xokiko'ob catalan", "chino xíiw",
            "ingles", "frances", "japones", "káastelan", "maaya yucateco",
        };

        private static LanguageMap _global;

        public LanguageMap()
        {
            Add("ar", "Arabic");
            Add("bs-Latn", "Bosnian");
            Add("bg", "Bulgarian");
            Add("ca", "Catalan");
            Add("zh-CHS", "Chinese Simplified");
            Add("zh-CHT", "Chinese Traditional");
            NamesToNames.Add("Chinese", new string[] { "Simplified", "Traditional" });

            Add("hr", "Croatian");
            Add("cs", "Czech");
            Add("da", "Danish");
            Add("nl", "Dutch");
            Add("en", "English");
            Add("et", "Estonian");
            Add("fi", "Finnish");
            Add("fr", "French");
            Add("de", "German");
            Add("el", "Greek");
            Add("ht", "Haitian Creole");
            NamesToCodes.Add("Creole", "ht");
            Add("he", "Hebrew");
            Add("hi", "Hindi");
            Add("mww", "Hmong Daw");
            NamesToCodes.Add("Hmong", "mww");
            NamesToCodes.Add("Daw", "mww");

            Add("hu", "Hungarian");
            Add("id", "Indonesian");
            Add("it", "Italian");
            Add("ja", "Japanese");
            Add("sw", "Kiswahili");
            Add("ko", "Korean");
            Add("tlh", "Klingon");
            Add("lv", "Latvian");
            Add("lt", "Lithuanian");
            Add("ms", "Malay");
            Add("mt", "Maltese");
            Add("no", "Norwegian");
            Add("fa", "Persian");
            Add("pl", "Polish");
            Add("pt", "Portuguese");
            Add("otq", "Querétaro Otomi");
            NamesToCodes.Add("Querétaro", "otq");
            NamesToCodes.Add("Otomi", "otq");

            Add("ro", "Romanian");
            Add("ru", "Russian");
            Add("sr-Cyrl", "Serbian Cyrillic");
            Add("sr-Latn", "Serbian Latin");
            NamesToNames.Add("Serbian", new string[] { "Cyrillic", "Latin" });

            Add("sk", "Slovak");
            Add("sl", "Slovenian");
            Add("es", "Spanish");
            Add("sv", "Swedish");
            Add("th", "Thai");
            Add("tr", "Turkish");
            Add("uk", "Ukrainian");
            Add("ur", "Urdu");
            Add("vi", "Vietnamese");
            Add("cy", "Welsh");
            Add("yua", "Yucatec Maya");
            NamesToCodes.Add("yucatec", "yua");
            NamesToCodes.Add("maya", "yua");
        }

        public static LanguageMap Global
        {
            get
            {
                if (_global == null)
                {
                    _global = new LanguageMap();
                }

                return _global;
            }
        }

        public Dictionary<string, string> CodesToNames { get; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public Dictionary<string, string> NamesToCodes { get; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public Dictionary<string, string[]> NamesToNames { get; } = new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);

        public string GetCodeForInput(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                if (this.NamesToCodes.TryGetValue(value, out var code))
                {
                    return code;
                }

                if (this.NamesToNames.ContainsKey(value))
                {
                    foreach (var name in this.NamesToNames[value])
                    {
                        var secondVal = values.Where(val => string.Compare(val, name, true) == 0).FirstOrDefault();
                        if (secondVal != null)
                        {
                            if (TryCompound(value, secondVal, out code))
                            {
                                return code;
                            }
                        }
                    }

                    if (TryCompound(value, this.NamesToNames[value].FirstOrDefault(), out code))
                    {
                        return code;
                    }
                }

                if (this.NamesToCodes.Values.Contains(value))
                {
                    return value;
                }
            }

            return null;
        }

        public string GetCodeOrFallback(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return "en";
            }

            code = code.Trim();
            if (CodesToNames.ContainsKey(code))
            {
                return code;
            }

            return "en";
        }

        private void Add(string code, string name)
        {
            CodesToNames.Add(code, name);
            NamesToCodes.Add(name, code);
        }

        private bool TryCompound(string firstVal, string secondVal, out string code)
        {
            var compound = $"{firstVal} {secondVal}";
            if (this.NamesToCodes.TryGetValue(compound, out code))
            {
                return true;
            }

            compound = $"{secondVal} {firstVal}";
            if (this.NamesToCodes.TryGetValue(compound, out code))
            {
                return true;
            }

            return false;
        }
    }
}
