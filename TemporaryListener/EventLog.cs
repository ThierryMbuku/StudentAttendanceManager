//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TemporaryListener
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventLog
    {
        public int Id { get; set; }
        public int EventTypeId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UserId { get; set; }
        public int EventSeverityId { get; set; }
        public string MetaData { get; set; }
    
        public virtual EventSeverity EventSeverity { get; set; }
        public virtual EventType EventType { get; set; }
    }
}