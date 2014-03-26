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
FTS|0|9568180";

            var doc = hl7Parser.Parse(hl7);

            hl7 = @"MSH|^~\&|OAZIS||BIZTALK||20140318115439||ADT^A25|09568240|P|2.3||||||ASCII
EVN|A25|20140318115439||||201403182330
PID|1||3410131027|34101315545^^^^NN|Vanhubulcke^Victor^^^Meneer|Vermeersch^Marten|19341015|M|||Pereboomstraat 15^^STADEN^^8880^BE^H||01/56 65 99^^PH~0494/71 66 14ei^^CP||NL|M||80398500^^^^VN|0000000000|34101315545||||||BE||||N
PD1||||003809^MATTELIN^GUY||||||||N
PV1|1|O|5551^001^01^WIL|NULL|||003809^MATTELIN^GUY||000573^Bourgeois^Karel|1851|||||||000573^Bourgeois^Karel|0|80398500^^^^VN|3^20140318||||||||||||||||1|1||D|||||201403180830|201403181140";

            doc = hl7Parser.Parse(hl7);
        }
    }
}
