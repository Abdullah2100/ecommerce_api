using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

public class User : GeneralShredInfo
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsBlocked { get; set; } = false;

    public string? DeviceToken { get; set; } = null;

    //1 :normal user ; 0: is admin
    public bool IsUser { get; set; } = true;
    public string? Thumbnail { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>(0); // this for admin adding many payement type

    public virtual Store? Store { get; set; } = null;
    public virtual Delivery? Delivery { get; set; } = null;
}