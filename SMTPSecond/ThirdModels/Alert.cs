﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SMTPSecond.ThirdModels
{
    public partial class Alert
    {
        public long Id { get; set; }
        public int AlertTypeLookupId { get; set; }
        public long FleetId { get; set; }
        public long? WarehouseId { get; set; }
        public long? InventoryId { get; set; }
        public long? SensorId { get; set; }
        public string AlertTextAr { get; set; }
        public string AlertTextEn { get; set; }
        public string AlertForValueAr { get; set; }
        public string AlertForValueEn { get; set; }
        public DateTime AlertDateTime { get; set; }
        public bool IsDismissed { get; set; }
        public long CustomAlertId { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual CustomAlert CustomAlert { get; set; }
    }
}
