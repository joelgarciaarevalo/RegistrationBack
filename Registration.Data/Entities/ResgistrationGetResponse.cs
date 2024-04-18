using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Data.Entities
{
    public class RegistrationGetResponse
    {
        public int EventLogId { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }

    }
}
