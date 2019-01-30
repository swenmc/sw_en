﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Integration
{
    public static class GaussPoints
    {
        public static double[] GetGaussianValues(int samplingCount)
        {
            double[] buf;

            switch (samplingCount)
            {
                case 1:
                    buf = new double[]
                    {
                        0
                    };
                    break;
                case 2:
                    buf = new double[]
                    {
                        -0.5773502691896257645091487805019574556476017512701268760186023264839776723029333456937153955857495252252087138051355676766566483649996508262705518373647912161760310773007685273559916067003615583077550051041144223011076288835574182229739459904090157105534559538626730166621791266197964892168,
                        0.5773502691896257645091487805019574556476017512701268760186023264839776723029333456937153955857495252252087138051355676766566483649996508262705518373647912161760310773007685273559916067003615583077550051041144223011076288835574182229739459904090157105534559538626730166621791266197964892168
                    };
                    break;
                case 3:
                    buf = new double[]
                    {
                        0,
                        -0.774596669241483377035853079956479922166584341058318165317514753222696618387395806703857475371734703583260441372189929402637908087832729923135978349224240702213750958202698716256783906245777858513169283405612501838634682531972963691092925710263188052523534528101729260090115562126394576188,
                        0.774596669241483377035853079956479922166584341058318165317514753222696618387395806703857475371734703583260441372189929402637908087832729923135978349224240702213750958202698716256783906245777858513169283405612501838634682531972963691092925710263188052523534528101729260090115562126394576188
                    };
                    break;
                case 4:
                    buf = new double[]
                    {
                        -0.3399810435848562648026657591032446872005758697709143525929539768210200304632370344778752804355548115489602395207464932135845003241712491992776363684338328221538611182352836311104158340621521124125023821932864240034767086752629560943410821534146791671405442668508151756169732898924953195536,
                        0.3399810435848562648026657591032446872005758697709143525929539768210200304632370344778752804355548115489602395207464932135845003241712491992776363684338328221538611182352836311104158340621521124125023821932864240034767086752629560943410821534146791671405442668508151756169732898924953195536,
                        -0.8611363115940525752239464888928095050957253796297176376157219209065294714950488657041623398844793052105769209319781763249637438391157919764084938458618855762872931327441369944290122598469710261906458681564745219362114916066097678053187180580268539141223471780870198639372247416951073770551,
                        0.8611363115940525752239464888928095050957253796297176376157219209065294714950488657041623398844793052105769209319781763249637438391157919764084938458618855762872931327441369944290122598469710261906458681564745219362114916066097678053187180580268539141223471780870198639372247416951073770551
                    };
                    break;
                case 5:
                    buf = new double[]
                    {
                        0,
                        -0.5384693101056830910363144207002088049672866069055599562022316270594711853677552910358036672505709315713670572321043495510816912158744046420683486075627481533978123828583369317846132387526796166796502053799563629878671716361660767584852200097418079241406256057571019602720019270523093750336,
                        0.5384693101056830910363144207002088049672866069055599562022316270594711853677552910358036672505709315713670572321043495510816912158744046420683486075627481533978123828583369317846132387526796166796502053799563629878671716361660767584852200097418079241406256057571019602720019270523093750336,
                        -0.9061798459386639927976268782993929651256519107625308628737622865437707949166868469411429895535422619115836248167051160932020660084349721915374869570125418659061700540273012086530604091207821562942704193786707298217315368769002376029537907738935528847397895557648103916797868140600953498906,
                        0.9061798459386639927976268782993929651256519107625308628737622865437707949166868469411429895535422619115836248167051160932020660084349721915374869570125418659061700540273012086530604091207821562942704193786707298217315368769002376029537907738935528847397895557648103916797868140600953498906
                    };
                    break;
                case 6:
                    buf = new double[]
                    {
                        0.6612093864662645136613995950199053470064485643951700708145267058521834966071431009442864037464614564298883716392751466795573467722253804381723198010093367423918538864300079016299442625145884902455718821970386303223620117352321357022187936189069743012315558710642131016398967690135661651261150514997832,
                        -0.6612093864662645136613995950199053470064485643951700708145267058521834966071431009442864037464614564298883716392751466795573467722253804381723198010093367423918538864300079016299442625145884902455718821970386303223620117352321357022187936189069743012315558710642131016398967690135661651261150514997832,
                        -0.2386191860831969086305017216807119354186106301400213501813951645742749342756398422492244272573491316090722230970106872029554530350772051352628872175189982985139866216812636229030578298770859440976999298617585739469216136216592222334626416400139367778945327871453246721518889993399000945408150514997832,
                        0.2386191860831969086305017216807119354186106301400213501813951645742749342756398422492244272573491316090722230970106872029554530350772051352628872175189982985139866216812636229030578298770859440976999298617585739469216136216592222334626416400139367778945327871453246721518889993399000945406150514997832,
                        -0.9324695142031520278123015544939946091347657377122898248725496165266135008442001962762887399219259850478636797265728341065879713795116384041921786180750210169211578452038930846310372961174632524612619760497437974074226320896716211721783852305051047442772222093863676553669179038880252326771150514997832,
                        0.9324695142031520278123015544939946091347657377122898248725496165266135008442001962762887399219259850478636797265728341065879713795116384041921786180750210169211578452038930846310372961174632524612619760497437974074226320896716211721783852305051047442772222093863676553669179038880252326771150514997832
                    };
                    break;
                case 7:
                    buf = new double[]
                    {
                        0,
                        0.4058451513773971669066064120769614633473820140993701263870432517946638132261256553283126897277465877652867586660480186780142389774087899602458293459431152403705864850136028192946798646997494188869169765542654505357384603100658598476270710450994883480024599267113885472679490162043321422574150514997832,
                        -0.4058451513773971669066064120769614633473820140993701263870432517946638132261256553283126897277465877652867586660480186780142389774087899602458293459431152403705864850136028192946798646997494188869169765542654505357384603100658598476270710450994883480024599267113885472679490162043321422574150514997832,
                        -0.7415311855993944398638647732807884070741476471413902601199553519674298746721805137928268323668632470596925180931120142436000543982298353471703857152740498332960747607976107150698769026932844561958151246095962171815950287169821619140709720118875391555834601955414971467103462901278094572097150514997832,
                        0.7415311855993944398638647732807884070741476471413902601199553519674298746721805137928268323668632470596925180931120142436000543982298353471703857152740498332960747607976107150698769026932844561958151246095962171815950287169821619140709720118875391555834601955414971467103462901278094572093150514997832,
                        -0.949107912342758524526189684047851262400770937670617783548769103913063330354840140805730770027925724144300739666995216194195625811353553118277789915859810085013901000179888247732305040104815148851112904940437420579459979108498442397952261081440138823188704950068274774322776063669713039873415051499783203,
                        0.949107912342758524526189684047851262400770937670617783548769103913063330354840140805730770027925724144300739666995216194195625811353553118277789915859810085013901000179888247732305040104815148851112904940437420579459979108498442397952261081440138823188704950068274774322776063669713039873415051499783203
                    };
                    break;
                case 8:
                    buf = new double[]
                    {
                        -0.1834346424956498049394761423601839806667578129129737823171884736992044742215421141160682237111233537452676587642867666089196012523876865683788569995160663568104475551617138501966385810764205532370882654749492812314961247764619363562770645716456613159405134052985058171969174306064445289638150514997832,
                        0.1834346424956498049394761423601839806667578129129737823171884736992044742215421141160682237111233537452676587642867666089196012523876865683788569995160663568104475551617138501966385810764205532370882654749492812314961247764619363562770645716456613159405134052985058171969174306064445289638150514997832,
                        -0.5255324099163289858177390491892463490419642431203928577508570992724548207685612725239614001936319820619096829248252608507108793766638779939805395303668253631119018273032402360060717470006127901479587576756241288895336619643528330825624263470540184224603688817537938539658502113876953598879150514997832,
                        0.5255324099163289858177390491892463490419642431203928577508570992724548207685612725239614001936319820619096829248252608507108793766638779939805395303668253631119018273032402360060717470006127901479587576756241288895336619643528330825624263470540184224603688817537938539658502113876953598879150514997832,
                        -0.7966664774136267395915539364758304368371717316159648320701702950392173056764730921471519272957259390191974534530973092653656494917010859602772562074621689676153935016290342325645582634205301545856060095727342603557415761265140428851957341933710803722783136113628137267630651413319993338002150514997832,
                        0.7966664774136267395915539364758304368371717316159648320701702950392173056764730921471519272957259390191974534530973092653656494917010859602772562074621689676153935016290342325645582634205301545856060095727342603557415761265140428851957341933710803722783136113628137267630651413319993338002150514997832,
                        -0.960289856497536231683560868569472990428235234301452038271639777372424897743419284439438959263312268310424392817294176210238958155217128547937364220490969970043398261832663734680878126355334692786735966348087059754254760392931853386656813286884261347489628923208763998895240977248938732425615051499783203,
                        0.960289856497536231683560868569472990428235234301452038271639777372424897743419284439438959263312268310424392817294176210238958155217128547937364220490969970043398261832663734680878126355334692786735966348087059754254760392931853386656813286884261347489628923208763998895240977248938732425615051499783203
                    };
                    break;
                case 9:
                    buf = new double[]
                    {
                        0,
                        -0.8360311073266357942994297880697348765441067181246759961043719796394550068815901188939461970258575402563758103910561868767921700399852813493611963795348388298072683628655858714286307690921827503279179493378017903390282931287792638170061442346288416366768259295268522725491437592698775616386150514997832,
                        0.8360311073266357942994297880697348765441067181246759961043719796394550068815901188939461970258575402563758103910561868767921700399852813493611963795348388298072683628655858714286307690921827503279179493378017903390282931287792638170061442346288416366768259295268522725491437592698775616386150514997832,
                        -0.9681602395076260898355762029036728700494048004919253295500233118490803743966007530618737492268941116024875911233178159906522811969602509341080006111457157352577320594030742939105200742221799581448832412180479160165668557217628253178605064255816845030589843605433053781978726946425719821479150514997832,
                        0.9681602395076260898355762029036728700494048004919253295500233118490803743966007530618737492268941116024875911233178159906522811969602509341080006111457157352577320594030742939105200742221799581448832412180479160165668557217628253178605064255816845030589843605433053781978726946425719821479150514997832,
                        -0.3242534234038089290385380146433366085719562607369730888270474768421865795351242491930986016984975672077778257173507373911718045575238432394572865005705333805025491599132630235053630398924931286361909328940173345187813296193687231694926973637651870715469270935223550274475117654585286698075150514997832,
                        0.3242534234038089290385380146433366085719562607369730888270474768421865795351242491930986016984975672077778257173507373911718045575238432394572865005705333805025491599132630235053630398924931286361909328940173345187813296193687231694926973637651870715469270935223550274475117654585286698075150514997832,
                        -0.6133714327005903973087020393414741847857206049405646928728129422812673464910011985832400139035685845782334895968597685619397117528519746872458346040371559996202334828312987463516926466812888532978280620182027590531371274017229787367921934803381534015176954113597402763904697814697273286917150514997832,
                        0.6133714327005903973087020393414741847857206049405646928728129422812673464910011985832400139035685845782334895968597685619397117528519746872458346040371559996202334828312987463516926466812888532978280620182027590531371274017229787367921934803381534015176954113597402763904697814697273286917150514997832
                    };
                    break;

                default:
                    throw new ArgumentOutOfRangeException("samplingCount");
            }

            Debug.Assert(buf.Length == samplingCount);

            return buf;
        }

        public static double[] GetGaussianWeights(int samplingCount)
        {
            double[] buf;

            switch (samplingCount)
            {
                case 1:
                    buf = new double[] {2.0};
                    break;
                case 2:
                    buf = new double[]
                    {
                        1.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000,
                        1.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
                    };
                    break;
                case 3:
                    buf = new double[]
                    {
                        0.8888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888889,
                        0.5555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555556,
                        0.5555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555556
                    };
                    break;
                case 4:
                    buf = new double[]
                    {
                        0.6521451548625461426269360507780005927646513041661064595074706804812481325340896482780162322677418404902018960952364978455755577496740182191429757016783303751407135229556360801973666260481564013273531860737119707353160256000107787211587578617532049337456560923057986412084590467808124974086,
                        0.6521451548625461426269360507780005927646513041661064595074706804812481325340896482780162322677418404902018960952364978455755577496740182191429757016783303751407135229556360801973666260481564013273531860737119707353160256000107787211587578617532049337456560923057986412084590467808124974086,
                        0.3478548451374538573730639492219994072353486958338935404925293195187518674659103517219837677322581595097981039047635021544244422503259817808570242983216696248592864770443639198026333739518435986726468139262880292646839743999892212788412421382467950662543439076942013587915409532191875025701,
                        0.3478548451374538573730639492219994072353486958338935404925293195187518674659103517219837677322581595097981039047635021544244422503259817808570242983216696248592864770443639198026333739518435986726468139262880292646839743999892212788412421382467950662543439076942013587915409532191875025701
                    };
                    break;
                case 5:
                    buf = new double[]
                    {
                        0.5688888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888889,
                        0.4786286704993664680412915148356381929122955533431415399727276673338382671525124569755621250616041107794464209474122299742927901670531874220236019762755381069981020199559708433435017355341690324695622104863536721598859262913644828482640505637133513606531929893286127565185389732581634388813,
                        0.4786286704993664680412915148356381929122955533431415399727276673338382671525124569755621250616041107794464209474122299742927901670531874220236019762755381069981020199559708433435017355341690324695622104863536721598859262913644828482640505637133513606531929893286127565185389732581634388813,
                        0.2369268850561890875142640407199173626432600022124140155828278882217172884030430985799934304939514447761091346081433255812627653885023681335319535792800174485574535355995847122120538200213865230859933450692018833956696292641910727072915049918422041949023625662269427990370165822973921166843,
                        0.2369268850561890875142640407199173626432600022124140155828278882217172884030430985799934304939514447761091346081433255812627653885023681335319535792800174485574535355995847122120538200213865230859933450692018833956696292641910727072915049918422041949023625662269427990370165822973921166843
                    };
                    break;
                case 6:
                    buf = new double[]
                    {
                        0.3607615730481386075698335138377161116615218927467454822897392402371400378372617183209622019888193479431172091403707985898798902783643210707767872114085818922114502722525757771126000732368828591631602895111800517408136855470744824724861011832599314498172164024255867775267681999309503106873150514997832,
                        0.3607615730481386075698335138377161116615218927467454822897392402371400378372617183209622019888193479431172091403707985898798902783643210707767872114085818922114502722525757771126000732368828591631602895111800517408136855470744824724861011832599314498172164024255867775267681999309503106873150514997832,
                        0.4679139345726910473898703439895509948116556057692105353116253199639142016203981270311100925847919823047662687897547971009283625541735029545935635592733866593364825926382559018030281273563502536241704619318259000997569870959005334740800746343768244318081732063691741034162617653462927888917150514997832,
                        0.4679139345726910473898703439895509948116556057692105353116253199639142016203981270311100925847919823047662687897547971009283625541735029545935635592733866593364825926382559018030281273563502536241704619318259000997569870959005334740800746343768244318081732063691741034162617653462927888917150514997832,
                        0.1713244923791703450402961421727328935268225014840439823986354397989457605423401546479277054263886697521165220698744043091917471674621759746296492293180314484520671351091683210843717994067668872126692485569940481594293273570249840534338241823632441183746103912052391190442197035702977497812150514997832,
                        0.1713244923791703450402961421727328935268225014840439823986354397989457605423401546479277054263886697521165220698744043091917471674621759746296492293180314484520671351091683210843717994067668872126692485569940481594293273570249840534338241823632441183746103912052391190442197035702977497812150514997832
                    };
                    break;
                case 7:
                    buf = new double[]
                    {
                        0.4179591836734693877551020408163265306122448979591836734693877551020408163265306122448979591836734693877551020408163265306122448979591836734693877551020408163265306122448979591836734693877551020408163265306122448979591836734693877551020408163265306122448979591836734693877551020408163265306,
                        0.3818300505051189449503697754889751338783650835338627347510834510307055464341297083486846593440448014503146717645853573344928956776383837562443187566373816994263513750309425122069048082192405967657155458166140211350441276674773890998286086501179973611850420964132316867109449892362441359044150514997832,
                        0.3818300505051189449503697754889751338783650835338627347510834510307055464341297083486846593440448014503146717645853573344928956776383837562443187566373816994263513750309425122069048082192405967657155458166140211350441276674773890998286086501179973611850420964132316867109449892362441359044150514997832,
                        0.2797053914892766679014677714237795824869250652265987645370140326936188104305626768132409429011976187663233752133720515191356369795631199443713526578123368545563592025336909192643194833502493348267909279642862302108156020621692804889667628308214167200738365553989662119973317061306305537859150514997832,
                        0.2797053914892766679014677714237795824869250652265987645370140326936188104305626768132409429011976187663233752133720515191356369795631199443713526578123368545563592025336909192643194833502493348267909279642862302108156020621692804889667628308214167200738365553989662119973317061306305537859150514997832,
                        0.1294849661688696932706114326790820183285874022599466639772086387246552349720423087156254181629208450894844020016344278810653448938189044626496347079992610378540241163129175889369389737366325173870853629537936262051606784336186365336536081108973206126186723685959653665979209749176350897029150514997832,
                        0.1294849661688696932706114326790820183285874022599466639772086387246552349720423087156254181629208450894844020016344278810653448938189044626496347079992610378540241163129175889369389737366325173870853629537936262051606784336186365336536081108973206126186723685959653665979209749176350897029150514997832
                    };
                    break;
                case 8:
                    buf = new double[]
                    {
                        0.3626837833783619829651504492771956121941460398943305405248230675666867347239066773243660420848285095502587699262967065529258215569895173844995576007862076842778350382862546305771007553373269714714894268328780431822779077846722965535548199601402487767505928976560993309027632737537826127502150514997832,
                        0.3626837833783619829651504492771956121941460398943305405248230675666867347239066773243660420848285095502587699262967065529258215569895173844995576007862076842778350382862546305771007553373269714714894268328780431822779077846722965535548199601402487767505928976560993309027632737537826127502150514997832,
                        0.3137066458778872873379622019866013132603289990027349376902639450749562719421734969616980762339285560494275746410778086162472468322655616056890624276469758994622503118776562559463287222021520431626467794721603822601295276898652509723185157998353156062419751736972560423953923732838789657919150514997832,
                        0.3137066458778872873379622019866013132603289990027349376902639450749562719421734969616980762339285560494275746410778086162472468322655616056890624276469758994622503118776562559463287222021520431626467794721603822601295276898652509723185157998353156062419751736972560423953923732838789657919150514997832,
                        0.2223810344533744705443559944262408844301308700512495647259092892936168145704490408536531423771979278421592661012122181231114375798525722419381826674532090577908613289536840402789398648876004385697202157482063253247195590228631570651319965589733545440605952819880671616779621183704306688233150514997832,
                        0.2223810344533744705443559944262408844301308700512495647259092892936168145704490408536531423771979278421592661012122181231114375798525722419381826674532090577908613289536840402789398648876004385697202157482063253247195590228631570651319965589733545440605952819880671616779621183704306688233150514997832,
                        0.1012285362903762591525313543099621901153940910516849570590036980647401787634707848602827393040450065581543893314132667077154940308923487678731973041136073584690533208824050731976306575729205467961435779467552492328730055025992954089946676810510810729468366466585774650346143712142008566866150514997832,
                        0.1012285362903762591525313543099621901153940910516849570590036980647401787634707848602827393040450065581543893314132667077154940308923487678731973041136073584690533208824050731976306575729205467961435779467552492328730055025992954089946676810510810729468366466585774650346143712142008566866150514997832
                    };
                    break;
                case 9:
                    buf = new double[]
                    {
                        0.3302393550012597631645250692869740488788107835726883345930964978584026203073822121441169060216679264298311917359536407155454774502393550012597631645250692869740488788107835726883345930964978584026203073822121441169060216679264298311917359536407155454774502393550012597631645250692869740489,
                        0.1806481606948574040584720312429128095143378217320404844983359064713572905449462697645949773031997041476074679602577937226796268460630127231790100804745577374812973964868278705556370432288860477148539230329025541102198218481213990057413494800065234875808239968200871271576666111786816983312150514997832,
                        0.1806481606948574040584720312429128095143378217320404844983359064713572905449462697645949773031997041476074679602577937226796268460630127231790100804745577374812973964868278705556370432288860477148539230329025541102198218481213990057413494800065234875808239968200871271576666111786816983312150514997832,
                        0.081274388361574411971892158110523650675661720782410750711107676880686686308452062945578554702942576957794073317963038094590048795093955759528141378844750853767333972349567507324558127938133868301667395157245896802611234739695631672003334674766636592975299135275084311484994311087346192507215051499783203,
                        0.081274388361574411971892158110523650675661720782410750711107676880686686308452062945578554702942576957794073317963038094590048795093955759528141378844750853767333972349567507324558127938133868301667395157245896802611234739695631672003334674766636592975299135275084311484994311087346192507215051499783203,
                        0.3123470770400028400686304065844436655987548612619046455540111655991438973240193165701219218880063538522954773181646973116391818098875271459600370901478405885572589090757645984059641355722376816546561522245422024969266380802745127735793790292136245228820749357799614002097074181144513901973150514997832,
                        0.3123470770400028400686304065844436655987548612619046455540111655991438973240193165701219218880063538522954773181646973116391818098875271459600370901478405885572589090757645984059641355722376816546561522245422024969266380802745127735793790292136245228820749357799614002097074181144513901973150514997832,
                        0.2606106964029354623187428694186328497718402044372999519399970021196108156688912446476460930950174018273873855356376505133184038238358268707029298682703161767070852826824482373696733967124934731275123758942032745317892944979452416330800688391928576238230768124473665313152599422632809847998150514997832,
                        0.2606106964029354623187428694186328497718402044372999519399970021196108156688912446476460930950174018273873855356376505133184038238358268707029298682703161767070852826824482373696733967124934731275123758942032745317892944979452416330800688391928576238230768124473665313152599422632809847998150514997832
                    };
                    break;

                default:
                    throw new ArgumentOutOfRangeException("samplingCount");
            }

            Debug.Assert(buf.Length == samplingCount);

            return buf;
        }
    }
}