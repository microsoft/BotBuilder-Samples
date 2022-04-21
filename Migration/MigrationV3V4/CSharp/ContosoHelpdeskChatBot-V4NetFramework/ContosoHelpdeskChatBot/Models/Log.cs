// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoHelpdeskChatBot.Models
{
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
