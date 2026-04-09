using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Common.Enums;

public enum OrderStatus
{
    Pending = 0,
    Approved = 1,   
    Rejected = 2,     
    Baking = 3,      
    ReadyForPickup = 4,
    Completed = 5,      
    Cancelled = 6       
}