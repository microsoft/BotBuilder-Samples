namespace ContosoHelpdeskChatBot.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Log")]
    public partial class Log
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }
    }
}
