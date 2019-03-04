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
            Polynoms.ModularPolynom a = new Polynoms.ModularPolynom("x^12",5);
            Polynoms.ModularPolynom b = new Polynoms.ModularPolynom("x^2+x+2",5);
            Console.WriteLine($"a(x) = {a}\nb(x) = {b}");
            Console.WriteLine($"a/b = {a/b}");
            Polynoms.ModularPolynom c = a % b;
            Console.WriteLine($"a % b = {c}");
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

