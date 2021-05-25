// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoHelpdeskChatBot.Models
{
    [Table("AppMsi")]
    public partial class AppMsi
    {
        [Key]
        public int Id { get; set; }

        public string AppName { get; set; }

        public string MsiPackage { get; set; }
    }
}
