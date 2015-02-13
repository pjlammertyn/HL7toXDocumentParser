using HL7toXDocumentParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var hl7Parser = new Parser();

            var hl7 = @"FHS|^~\&|OAZIS||||20140318114932||ADT||9568180
MSH|^~\&|OAZIS||||20140318114932||ADT^A28|09568180|P|2.3||||||ASCII
EVN|A28|20140318114932||||19900101
PID|1||3612304001||Caltleau^Monu^^^Mevrouw||19361230|F|||Residence Charles 477^^ORCUE^^7101^BE^H^^^Y||||NL^^^NL|0||||||||||BE||||N
PD1||||||||||||N  
PV1|1|N|||||||||||||||||||||||||||||||||||||N
ZIT|1|2|3|CA^2.0000^257.6600~EA^1.0000^128.8300|1094
FTS|0|9568180";

            var doc = hl7Parser.Parse(hl7);

            hl7 = @"MSH|^~\&|OAZIS||BIZTALK||20140318115439||ADT^A25|09568240|P|2.3||||||ASCII
EVN|A25|20140318115439||||201403182330
PID|1||3410131027|34101315545^^^^NN|Vanhubulcke^Victor^^^Meneer|Vermeersch^Marten|19341015|M|||Pereboomstraat 15^^STADEN^^8880^BE^H||01/56 65 99^^PH~0494/71 66 14ei^^CP||NL|M||80398500^^^^VN|0000000000|34101315545||||||BE||||N
PD1||||003809^MATTELIN^GUY||||||||N
PV1|1|O|5551^001^01^WIL|NULL|||003809^MATTELIN^GUY||000573^Bourgeois^Karel|1851|||||||000573^Bourgeois^Karel|0|80398500^^^^VN|3^20140318||||||||||||||||1|1||D|||||201403180830|201403181140";

            doc = hl7Parser.Parse(hl7);


            hl7 = @"MSH|^~\&|DAVINCI|PATHOLOGIE|MEDIWEB|MEDIBASE|20141024141206||ORU^R01|20141024141206|P|2.4
PID||3005226074|30052280078||Margritte^Mariette||19390522|F
PV1|||||||||||||||||||80533628
OBR|||P14030638|PATHOL^Lymfeklier||20141016111758|20141016111758|||||||||||||||20141024113726||PATHOL|F|||000039^Vuylsteke^Pol||||000780&DEDEURWAERDER&FRANC
OBX||RP|PATHOL^Lymfeklier||\E\\E\agfalink\E\davinci\E\DOC\E\P14030638_20141024141206.doc||||||F|||20141024113726||000780^DEDEURWAERDER^FRANC";

            doc = hl7Parser.Parse(hl7);

            Console.WriteLine((from elem in doc.Descendants("OBX.5") select elem.Value).FirstOrDefault());

            hl7 = @"MSH|^~\&|Medibase|Medibase|Mediweb|Medibase|201410241106||ORU^R01|/ 24-10|P|2.3
PID||4805191763|4805188863||BOSSU^Wanny||19440719||||||||||||||||||||||||
OBR|||MDB000000019620|MDB^MediBase brieven|||201410241200|||||||||13226543884^DUMONT^FILIP^^13226543994^Dr.||||||||ORL||||13226543884^DUMONT^FILIP^^13226543994^Dr.||||000287^Dr. Devos^F.^^13998876414^Dr.|||||
OBX||FT|MDB^MediBase brieven||Geachte collega,\.br\\.br\Ik zag uw patiënt, BOSSU Wanny (°19/07/44), op de raadpleging op 24/10/2014.\.br\\.br\Persoonlijke antecedenten\.br\- arteriele hypertensie\.br\- 20/04/2009: tympanoplastie type II owv cholesteatoom rechts\.br\- 1/2012: geleidingsverlies rechts door disclocatie titaniumprothese\.br\\.br\Gevolgde medicatie\.br\Emconcor mitis 5 mg, Zurcale 20 mg, Pantoprazole\.br\\.br\Anamnese\.br\Patiënt komt voor de bespreking van de CT beelden.\.br\\.br\Klinisch onderzoek\.br\Micro-otoscopie: rechts intacte, rustige fascia greffe. Links gekende posterieure perforatie; momenteel rustig. Intact, doch verfijnd incudostapediaal gewricht.\.br\\.br\Audiogram\.br\Fletcher index R = 88 dB - L = 60 dB\.br\RIZIV index R = 93 dB - L = 68 dB\.br\IPA index R = 85 dB - L = 60 dB\.br\BIAP index R = 94 dB - L = 64 dB\.br\Rechts gekend gemengd gehoorsverlies, dat wel toegenomen is.\.br\Links gekende presbyacusie met geleidingsgehoorsverlies van 30db \.br\\.br\Besluit\.br\De CT beelden zijn uitermate suggestief voor een recidief cholesteatoom rechts.  De prothese is duidelijk anterieurwaarts verplaatst, wat de toename van het gehoorverlies rechts verklaart. Dit oor zou dus om pathologische redenen opnieuw moeten gesaneerd worden en dit liefst vooraleer er opnieuw otorroe enz. ontstaat. Links is er een periode van otorroe geweest, doch dit is opnieuw afgekoeld. Als we dit in de toekomst willen vermijden, zou het best zijn de perforatie links te sluiten.  Het lijkt me echter geen goed idee aan zijn enige functionele oor te werken vooraleer het rechter oor gesaneerd en liefst functioneel verbeterd is.\.br\Patiënt gaat hierover nadenken: ik meen begrepen te hebben dat hij niet zo bevreesd is voor de ingreep, maar wel voor de algemene anesthesie. Hij zal dit met u komen bespreken.\.br\\.br\\.br\Met vriendelijke collegiale groeten en dank voor Uw vertrouwen,\.br\\.br\\.br\\.br\                        Dr.med. Dr.sc. F. Devos\.br\                        Otologie \T\ Neuro-otologie\.br\                        Revalidatie-arts|||||||||20141024||||";

            doc = hl7Parser.Parse(hl7);

            Console.WriteLine((from elem in doc.Descendants("OBX.5") select elem.Value).FirstOrDefault());

            hl7 = @"MSH|^~\&|Medibase|Medibase|Mediweb|Medibase|201410241106||ORU^R01|/ 24-10|P|2.3
PID||4805191763|4805188863||BOSSU^Wanny||19440719||||||||||||||||||||||||
OBR|||MDB000000019620|MDB^MediBase brieven|||201410241200|||||||||13226543884^DUMONT^FILIP^^13226543994^Dr.||||||||ORL||||13226543884^DUMONT^FILIP^^13226543994^Dr.||||000287^Dr. Devos^F.^^13998876414^Dr.|||||
OBX||FT|MDB^MediBase brieven||C:\E\temp\E\test.doc |||||||||20141024||||";

            doc = hl7Parser.Parse(hl7);

            Console.WriteLine((from elem in doc.Descendants("OBX.5") select elem.Value).FirstOrDefault());

            Console.ReadLine();
        }
    }
}
