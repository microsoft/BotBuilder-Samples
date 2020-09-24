using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catering.Models
{
    public class CardOptions
    {
        public int? nextCardToSend { get; set; }
        public int? currentCard { get; set; }
        public string option { get; set; }
        public string custom { get; set; }
    }
}
