// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoHelpdeskChatBot.Models
{
    [Table("InstallApp")]
    public partial class InstallApp
    {
        [Key]
        public int Id { get; set; }

        public string AppName { get; set; }

        public string MachineName { get; set; }
    }
}
