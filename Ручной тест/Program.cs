﻿using KMZILib;

using System;
using System.IO;
using System.Linq;
using System.Text;
using Ciphers = KMZILib.Ciphers;
using Polynoms = KMZILib.Polynoms;

namespace Ручной_тест
{
    class Program
    {
        public static string cipher =
            "ХТЕУФФТШЭЭАЩНРБСЯХЫЩФЦАЖСЦЪЕФСАНРЬЕРФЦЪУТСЕБЭЮАЮМНГЧЮМЬЕЧООБСАЯСХДБИЛОАДЪЯТТРОФСПЧЯБНЁЧТЩРДНЭЮЕЭФЦАЙЪЯНЪЯЬАТЦТАНЪЩТФФЭАЮЦИЗЖТЪЦЙШЛТБСДЭУНЁЖНВФЕЮЫЬФХХСЮНУЪВРЫЙЯУФЪЖНЦЛЦЩСЪЧЧЯХТЗЛЯСВИСЮДБЦКЙЩФЯВСЯЕЙЭЭЫТРХНЛЮБГЕТЯЧТФХРХИЩЫМЯУТЧЗЛЛУЧЮЫЧЭУНЬФЭЛХЦЦСУАВЧИРЧИГНЩСФБЪВУШЪЩЩХТЕУФСХЕЯЭСРЩНДЦЗЪЕЕЮЗПЯЛЮЧЭЮАДЧФЫОЬЬЕЧТЮМЗМЩЛЧХЦЦЬФБАЦЙШЪНВЫЮЫХЯУТЧЗЬАСЧАНФБАХХЛШМЛЦНЕЙВДБЖЪХСЩЪАЧСХЭЫТРХНЛЮБГЕТЯЧТФХСЩЪАЧСРЭЫТРХНЛЮБГЕТЯЧТФХСЩЪАЧСРУЫИЛНОСВТЯЦЬЕЙЕРЮАЫИМДНВДЧСЛШОЧНААЙЩЮЧЧЗЬЕИНЫЫОТАФЦРФМХЫЙБИЮАГЙДРНЩНЦЯЙГНУЙЭЦОЮНЕАУХЮАУТРСГКЬГЙИЦАНХПАЬНТЯАСЕЦЙШЬАВЪЪТЧБЪФЕЮЗВРЩЦТТГЬБЗПЛСЩЪАЧСРЪЯЙРЮОФЦЬГЙИЦАНРЮОХЪАОПЯФЦЕЪЮОЧМТДЧТЪЯУМЛЭГСГБЁЯЩАЕГРНЩЗОЁПТАЬСРЮОФЛНЁЦВСВХКШОЪИЩХУБЪЕСЬРШХЦЦСЦЩГЕЙШЧИЮНЧААЁЕГЕНЩЕЮСЧДУВЭБЛЩЪСГЕЛХИХВТМШРРЮЧЯЕБЩГЗУПЩОЪХНЕХЩЗНВЮЪТРФРБХЩДЯШПЪБЮЧЦДФЯЭОМЪНАГЕШБЗФСГЕУШЕБХНЕХЩЗНТРНЕЬСШТЦРЮТСЛЮРЩЯНСЖЬРЧЧЭКПЬЧААУШСВУЧЩЕЮЦЬЬФБЪВУШЪЩЩЁАБЗЯЮЧЧЪПЫЭЧФАУЩГЭЕЮЗОАЩТЦЙЬЪЕЙЧЗМРЫЮЫЫЛСЕЕЦТЕЯЙЮТЧЮАЬПЩРЙЭИАГНЖМЦЕЩЩЫЪИЩХУБЪЕСПРЛЩЫЯСТРХФЙГЛСГСЭГДЭАЬНЪМРРЫЫНОЁАЦАНЪВБНЪСФБРЯУОЪХЯМНЯЕГВЫЫЛЦОНЭВЫЫЩЦАЧЪНПБСПБИЩДДДЦЮРХЬРБРМЯБСЯНИФЬНФУТВЧСКЪББИААУУАВХЪФСЁЧСЫЧАВБЫРЭСТДЕЫЦЬЦАНКЪТФНЩОТЯФБПЪИФЕСДЫЙЮДТЕЧООБСАЯСХДБИЛОАДЪЯТСЯШАУЪЫИВИАОЦЬЦЦШЙЕИЭСЁТЗРЮЫФЯЭТМШТГЙЭЦАТЛКОГКТЙЕХДЪЕЮРКДВБРЦГВБПЯШАГЩЦИАЩБГУМРГРНАФЦХЩАЕГРНЩЗЬЕИЯФЦЙЬЛЗЭНЮАУВДОСЛЮРЩЯИУЙШЕЙЙЮЛСГЧЩУЫРГФУМЪДЮДГЭУНЁЖНВФЕЮЫЬФЧХЬЁЮЛКСГЩЬЭЕФЦЮНЮЭЯЮИКЮЙЭЦАЧПЧЯДМЬУЦГУТЖЖГИВФТАНПДЧУЬРТЩАТДПЩОЕУОЪМЯПЫБНЮЦЦЙЧЛТМЦЬПЧЯББСЪОАХЫПЦЕЬНАЙХДИЁКИЙНВЭЧТФКХЮЧПЮНПЦЕТЛЫОУЩТКТЯГЕБФЭКЬЖЕТЙЭОЮЙШРНГСХФЦХЖДЧЬЪКЭИАГНЖМАЕГФНРЗЯВУВЭЧИЪНАГНЩОТЯЕЯТЪТАПЬЛДЧБАЭШЩЛКЯЁВЖНЖЪЧТЮОДХШЮБЁХФТЙЮУНРАТАНПАЕИЪЪСГИЩОТЛЦЭУИАФЩЯЦЧТГМФАЭГИГДПТГГГСФЪЭЛХМЬФЕГЦЮБЩЪПЯЭЬГСДЭЧФЬФПБЧОЧЗРПЛЙШУНРАТАНПАЕИЪЬЕШЬЩОЧРДВХЪОРРХЪАУЪВЧЕЧФЗРЯЦЫЧРЬБЗЪЛЛУЧЮЫЧЭСВХРПСГИПЮЙЮЯТХФЭУЮУТГНВЕАУЦЫРЯЯТДЦТМВУЧЩЕЮСМВХПЮБЗЪБОФИЯЕЕЮХТХЮЩОУЧНЮЗЯВЫЧШЛГРЬЯДЕЮСПЖШПАЬНТАЙЯТЗУПФМЯШЮБЖХВЫЧЗЫОУЩТКТЯГЕБЫЪЛДАТАТЛЖГЙУЯЛМЫНЕУТУНЬФЭЛЩЫЙВУУВЧЭЩЪСГЕЭБРДИЧТЩЪГЯЩТКЙЮЪССЪТНЯШЬЦЦГСФННЫОЬЬЕЧТЮМЬЖРЦТЯЩПЯЕГВЫЬЩЪЕДЩНФТХЯЫЙЫЪЛДАЦЯТЯУНОНРКГЧЮЕЕЫЬТПШЖИЭНТЯНШЯТЬЛЧЬЮДЧФЙЫДБХШЖМЯПТЯЖЛГЙНЮЛТМЩНЪТЩЗЁСРТДДЦЦЯНЫСЭЗПРНЯЩЪТСРДГНВЖИЬСПЧПГАГЕЬРЗДФЙЕЕГДТПФБВЛАЦДРХЯЫОПЧЯАЧЩЁЬХЯААБУНРАТАНЪБГЙПЭТРКЩЧТЮСГНЭЯНЫНЮЫЦДЯБПЫЪГБНЁАУВДОФЪЧУЗНЫАУУАГЙУЯЛМЫНЕЕАВЫНЭЫОЬЕХБЖРЯЫНЭЮАЮМНГЧЮАХУЛЧГЯЩЦЕСРФТШЭЭАГНЭЧХМВТЦЭШОГЩЦЯСЯХЫЩФЦАЖСЦТРУАГНЮШАУИБДЦРУНЁЪЬНРСОБРМЙЧЗЪИЛХХТАЧРУДЧЬЪКХКИУУБЯТНМЪЛМБТХУНЭЧСРЩТРКЯЕУЬТИЙНЖБЯЩНАЕЩТБРЗДЕУЧКЮЙЭЦАЧЛНОТЪТЬСРДГНВРВВНКЕНЭАЦНАФКРЯЦЫЁЛЭЫЦЪУДРЦИЦРПЕЯЙЩЗШХЦЦСФЯФГЙДЩОВЫЦФАЗЪДРРЩИЪШЮЫНВББРЗУОТИЫЫНРЭХУЬФТЭИРТШВГТЖЭРОЮСХТПЬПЙЕЙЮСПКЭБИВДТТЪНКХЦНЫЁЯЭОЭРООНФТЯЙЮДТЁЯПЬГЧПДЧБАЭЙНЭТЯФОИЙЩЭЫЖЪНСХТЪТЧБЪИЙЩЛМХЪАБЧХЬЁЮРООДЛЩБЖЯФБВЧРМХЦАТЦГАЫЧУЛМХЫЦЕБЗДБМЛШЕЮИЪЧЦГСЯНЭЮРЯУПЯЕГВЫЫРЩЕЮНЯЧЧЮЪЭЕЦФХЩРЪЧТХЯЫОЩЛРХРБЮБГСЕЖЖГИВФТАНЪЯБМЛШЕЮИЯЕУЬТИУННЛХАТЕМРГБЁЪХИШХТАЙЮЪЧФЪЬЯФУНДРХХБЖЛЩИПУЬБХФЪАЕЮНРХРБЮБГЪГШЙЕЕЭКТЭЧЯВЧВЮЪПЯРПБРПЦЕУЮПАЬСАОХХЩЁРЗЮАГДЭБРДИТЙШЖХТДЕЫЦЬЦАНХЪТЮЬЩСНГЦЯЦЛШЫЭЬЪЧТМЙЫЧЗЫОУЩТКТЯГЕБЬРЗДФЙЕЕГМПЧФБАЬЛЬГНГЮБЖЫЬЕФЪАТЖЬЦААЩЛРЩЪБАПРЖЫЦЪЪТТНАДЧТЦАТЪЬИВЬЫБПБЦЪШЧЗТРЫНЮЗЯВЫЧШЛГРЬЯДЕВЮБИФАИЫИДЫЙЪУНЁЪЬААЧЯЕХЯЬЧХФЭУЮЧШГЙШЕЮБЮЛТРФРБХЩДЯЕОЛУВЪНДСЯХЫЩФЦАЖСТЬЖЛТБХЛЫОВЫЬЮЁЖЕГНЭЯНЯУЮЧМДЭОЧЛЮАЬЛЬГНГЮТЗЛЯСВИЯЯУФЪЖНЦЛЦЩНЧФАСАГЕЫЪВВНЧЯЕГВЫЫРЦАЫХЬЩТЯЩТСРЮИГЕЫТНЭЦАБДЯЮАЧРГЙИЯБЦЮЗИЭНЛЕСХДБИЖЭВЛЙЬГУЭЯТНШРНМБТХУНЭЧСРЩТРШЬДЧЯЭУЫРФПЯКЯЧОЭСЕХФВЕЗЫЬЮУУЪЙТЪЯЧЩЫИФЕПИЕУЫРРТДЧФЪЯХЫЧНЪВГЧЮБОГСЭЛРЫРЩШЬЯУЙЪЯЙЮЪДРЛНЁЦВСЯУТЩОТДЯЙНГСЕБЪЫРХМТЮНГЦЮБШЛТБСДНИЬРПЧЪООЮИШТЛФАЯЭЛОЕРФРБХЩДЯЕОЛУВЪНАЙЯТЗУПФМЯЬЕЧЦГНПРРШЕЮЫЫТЗЬСФТЪХДЩИРБТРЭЫННЫОВФТЦЦГУЫНФБВВНГВЙБЦЯТЪТИГЕЦААЭЪДРЪНАЭСЫЧУСЖБИФШОАЩЬЫМТЦДЧФЯМЮЧФЧТЩЦПРРШЕЮЫЬФЖАВБЫРЭСХКИВУЬЯЧТФКАЬЛЬГНГЮТЗЛЯСВИЮЧМДЭОЧЛЮЭГЧРБФБЦЦЦЮЛВЬНЫАЕБЪДШЩЦЕБСЯЁТЯЬГЙУЯЛМЫНЕТРЖБЛПРНЩЗЬВХХХЧРФЮЕЬЗЪТЧБЪИАЭФСАЧЩОМЯУТТФРМРФРБХЩДЯЕШРТЯМНХЕДГДЕШРТЯМХТИРЯЫЙЬРАЬСХБЖРДОСРЮОФЦНПЖЭХЮДЬРШХЦЦСЦЩГЕЙШЧИЮНЧААЁЕГЕНЩЕЮСЧЪЕАЪДЕЩЩЫЁКПЫИХЮТЧЬФЦЫЧКЖЩЩЗЫЙЩЮОТЩТТРЩЩБЖЛЮЬРФРБХЩДЯЖЖГИВФТАНПАВХРПЕЬСАЧРПБГНЫЪМЯВЦЯЙГАЦЕЬЛСВАЦЕЕГНВУОЬЕИЦЬДЧЩББРЯГЕЮЦИЗМЮСЙЙЩФЙАЩЬЦЙЭААЦЮЬИБЧПТЧМВТЁЪЮЫАЩЬХХРЮЯАЩЛИЭНЛЛЙЪГСЦФЭТХХТЮНЮЦЬТЖБУБИПАЙЮЪЬУПЩОЪСХЫМТЦДЧЩЖХЭЧСЫЩЩЬТЫФКМХЫЬЦЕУСЁЦЭЛЯТФМЧЧВРЯЙЮЪДЯЦХТПЬПЙЕРЮСПКЮТМЬАЩЙЩФИЩЪГБИЮАЬСЛЮРЩЯИАЕФУЧСЛЮРЩЯИЫЧРЬЫЪГЮОВЫЬЫЧШСЯЙЮФТМААБСРДГНВЛПЯМСЫЕУААЕЧЗНРЗНАЕФХЫЕОЪНРФЙАЕПЮТЧЬФЦЛЧЫЫНЭЦРЧЭЧЕФЬЛЛНЪУЫИМЖЛВЧХЦЕЮГЮЙПЯЮЙСЧТРУАГНЮШРРЪЕЧЧРЮТЧЬФЦЩСХАЕЗСЮБЩЪПХЩПТДВДГУЦЛМРЫЮЫЫЛББРЩЪСГЕЛДУВДБНЮФЗНФТЯЙЮДБЖЫРРТЧЧДЧБАЭНШЛТБСДНФХВФАХЭТЯФОЧЫЭСЕХФВЫШИЦДПЬПЙЙЩФЕЭЁЩЧСХЯЕЕЭЪСГЧЦЕНШОЮЙШРНГЧПЫЪЬЦХПЪНЫВАЦЕЕГНЫСРКНРЩБЭЕЁХТРРРСЬНСЁЙГЗЫПЧФЧХЪШЫОААЫЦЦИЛХХТАЧЯУЯЕЮЬИЖСЬАУАЪДАНЛЕГЪМДРХХЁГЕФМЩБНХЕЭЪВШЭЮЬАНЮЧСХЯАЕКЪТТНЕТЙГЩТЧРЦУЙЬЛДЧБАЭШШЛТБСДННАВБЁРОАХЫПДЙШЯТЬРЩИПЧАЦУУХЧХЛУМХЩЫБЦГНЯЕЮЬИЖДОЧМДИЧЧЛЭТЯФОИЕВУБЁЪПНЛЮШБВЕЁЫЫФРНГЧПДТРИТРЛНВЛАЦДРХЯЫДБЯЧРЪАФШОДФЦРИЛХХТАЧЛГЕХЪЦИАЧЩЁЬРЦЯУОЪУУЧЩЭЕЭСЕХФВЫТДЕЫЦЬРРЧЭКЭЬНЪЧТГМЕЙЫРРМКПНЬЩГЮЙЩФЯЁЬЕЁЖВДФШЙЮОВЫНФЭЩЦДДИЧЕЭНЫЕАЯФБЦЮЪЛСЯНФАЗЪДРКЙТВЗКЮЙЭЦАЧЖЬЕШЬЩОЧРДЕЕЦФХТДЕЫЦЬЦАНХЫРХМЯЕЕТЭЧТЩЛРЩЪБАПХВЫЦЯЩОЫЩТЪШЬНЕЕЮЫРЩХТАЙЮЪСЕЧООБСАЯЕЭЦЕУПЛСГЧЦЕМРЮЧЧФЮЬЗЫЬБФБЦЦЙЧФТХФЙЯЕГВЫЫЖНТРУЬЯЦЬЕЙЕРЬАТНЫБФБЦЦЙЧФТХФЛЯЕГВЫЫЖИТЯСХБЁБСЩЙЩЪНРЩЦДШЮЬЧХФЭУЮЧШГЙШЕЮБЮЛТТДЕЫЦЬЦАНКЪПБНСЧРЩДЧРКШАГЩЦИАЩЮБЛЩЪАЮИЩБЗЩИААШЪББИХБСЯЗЧТФЮЬАЧРГЙИЯБЦЮЗДРЦЫБЗЯЮЧЧЪПАНЫЬЫМЯТГЕТРНЯЦНГНВЕАПРЬИВЬЫБПААХХРДНЯЪАОХХСЮНУЪВРЦЫБЗЯЮЧЧЪПАЯЙЮТЧЮСССЛЮРЩЯНЪЕФСАНРЬЕРФЦЪУТСЕБЛЧГЯЩЦЕСЮСЗУТПЕЮСМБЁБСЕТЪХМРЫЮЫЫЛЬЦЕЩЩОЪШЮЫФЯЮБЮФЛЛУЧЮЫЧЭСЯЙЮЪДРЛНЁЦВСГЕЭЭЧЩЫНЕБААХХРДНЯЪАЫФЯЭЁЬРЩНЛЮХАЕЗЦАНХЫРЯМТЯУЮГЕХФЬОТИАОХРТБЧЖЫРЯЛЮТСЭМАЕФШЕОВТЬЦПГЫЦЮРМХФЦАЙЪЯНЪЯЬАТЦТАНЪЦДРФЬАВЪЪБЧБЦЕБШЛТБСЕАУХЕГЕНЩЕЮСТТМРДЧСЪЫРХМТЮЙЮЪЧУМЬАГЦЬЬСРДГНВЖТЯХЬЩТЯЩТСРЮИГЕЕЕУТВЧМЯЧЬГИАЧШЭЯБЛРЩИПХНЕХЩЗНТЛЦАЧМИЬЦГАЮЁРВМРЫЮЫЫЛЮНФЪЧУЗСЪАЕСАГЦЮЪЛСЯЬФСРДГНВЖТРУЦЯУСВТМЪШАЬЛЬГНГЮАЕБЪЖФНЫЫДЭСЕХФВЫВКЬЦНГГСПЩЛХЯПСЧТЩПФЦРБЕХЪАБРСЗБЖФУСЩЪАЧСЛЕГЕНЩЕЮСЧЦРПЯТЪЪТДХЦЦСХХЙЧТФХЭГЧЧДНВДЧСЖМУФНЪЫЦААЮБУЪВРЫЙЁЛХЪЯЙЙЕИЪЪМТРУАГНЮШМХЫЬЦЕУСЁЦЭЛЕВФЦВУЬЕЙЙЩЩЫХЪАБРСЗНЦЦЧЕЩЫЙЦХДФДИЬЯГЯХПБИЮЕЯЕЮЬИЖЬЪНФЯЭЁЬФШНДПЫЁГЭСЕХФВУБНХЁРМДТЧЮЛКЯЛЬТРУАГНЮШААЩЦФЙФЦАТЛЬИВЬЫЭЙБЪДШЩЪКАЧЩЁЬХЯАЕКЪББИААЕПЮТЧЬФЦРМЩСФБАФЙЬЦИАЧРГЙИЯБЦЮФМЯПЫБЖЯГВУЧЗЗЯКНЕБВРЁХЛННХЦЦЧСФЭСУВРНЩКНАНПББЗЬРШЮЧЯЕНСЕЦЙШФСАЧЩОМЯУТЧЗАОБХБЮШУХЧТЪЬМРХНЕХЩЗНХРУУЬЕАТЧТМЙНЭЧЕЮСМВУУВЧЭЩЪСГСЭГЙФГЕЕНЧЕЮЦНГНВЕАПРЬИВЬЫБПААХХРДНЯЪАОЖЛИЫЦЧРНЩЗЬУХРДАУХШАГЩЦИАЭЦЕУПЪМУИБДЦРУНЖЪППЯЛЮЧЭЮАДЧЗНЫЗСЯЮЙЮЪЬСЪТЕГЪЦЮБЮАВУНЧИПЫЙАЕБЦЪШЧЗТРЫИЯТЯФЫЪШЛТХХНЕНЗЦДПФБАЬЛЬГНГЮБЖФУАФИЕЦРПЪЪЁЛНЛХЦЦСУГЕДРЪННЛЮЭБЗБЦКТЪЭТХТЮТМБСУЕЮЖВРЖАДДЭЦЕУПЖИЭЧСЫЩЩЬТЫФФСДВТДЧТЕРЮФБМХЫЬЦУТББМНЪЛПЖЖЫЪВАЭХЛЮИГЕЕЫЦЬАБФРЬАЖСЧТЧРЬЩЙУЛМХЦЦЕБЩЖАЕМЪЛХНЫТИХШААРНПЬИЫЧЖЛИЫЦЧРНЩТНЮЗЯВЫЧШОАДЪЯТФДГЕБФКВЬЗТЕЦПХБЦЮЛТЯАЫБЧЯИААШЫРПХИЯСХДБИЪШРХБТАНПГЫЦЮРМЬСЫЧОЮМЗШЬЛВЮНЫЫДЮАБТЮЪЖХСЪЧЙГББЗЬРШЮЧЯЕБЩОЕШЫЪГБНЁАУВДОСЪТНЯЬЪЧТМЙЫЧЗЫРЩШЬЯУЙЪГЕУЧИЗЦИЗСЯХЫЩФЦАЖСЧТРУАГНЮША";
        static void Main(string[] args)
        {
            Polynoms.Polynom a = new Polynoms.Polynom("6x^3 -5x^2 +3x-1");
            Polynoms.Polynom b = new Polynoms.Polynom("3x^2+2");
            Console.WriteLine(a/b);
            Console.WriteLine(a % b);
            return;

            Ciphers.Languages.CurrentLanguage = Ciphers.Languages.RussianLanguage.GetInstanse();
            Console.WriteLine(string.Join(", ",Ciphers.VigenereCipher.KE.GetKEHypotheses(cipher)));
            string[] numbs = File.ReadAllLines(@"C:\Users\maior\Downloads\numbers.txt", Encoding.Default);

            double[] Test = numbs.Select(Convert.ToDouble).ToArray();

            MathStatistics.RandomValue value = new MathStatistics.RandomValue(Test);
            Console.WriteLine(value.DescriptiveStatistics);
        }


    }
}

