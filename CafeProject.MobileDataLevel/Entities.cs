// Поменял названия переменных и полей. =) не обижайся
// класс FoodTypes => FoodType (мн.ч. => ед.ч.)
// класс ObjectMenu => ObjectMenuItem
// для ясности

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeProject.MobileDataLevel.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("ID")]
        public Guid ID { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("MiddleName")]
        public string MiddleName { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Login")]
        public string Login { get; set; }

        [Column("ConfirmEmail")]
        public bool ConfirmEmail { get; set; }

        [Column("Gender")]
        public byte Gender { get; set; }

        [Column("Active")]
        public bool Active { get; set; }

        [Column("Deleted")]
        public bool Deleted { get; set; }

        public virtual Password Password { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }

    [Table("Passwords")]
    public class Password
    {
        [Key]
        [ForeignKey("User")]
        [Column("UserID")]
        public Guid UserID { get; set; }

        [Column("Password")]
        public byte[] PasswordHash { get; set; }

        [Column("PasswordConfirmed")]
        public bool PasswordConfirmed { get; set; }

        public virtual User User { get; set; }
    }

    [Table("Roles")]
    public class Role
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Role")]
        public string Title { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }

    [Table("Regions")]
    public class Region
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Region")]
        public string Title { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }

    [Table("Cities")]
    public class City
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey("Region")]
        [Column("RegionID")]
        public int RegionID { get; set; }

        [Column("City")]
        public string Title { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<District> Districts { get; set; }
    }

    [Table("Districts")]
    public class District
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey("City")]
        [Column("CityID")]
        public int CityID { get; set; }

        [Column("District")]
        public string Title { get; set; }

        public virtual City City { get; set; }
        public virtual ICollection<Street> Streets { get; set; }
    }

    [Table("Streets")]
    public class Street
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey("District")]
        [Column("DistrictID")]
        public int DistrictID { get; set; }

        [Column("Street")]
        public string Title { get; set; }

        public virtual District District { get; set; }
        public virtual ICollection<ObjectAddress> ObjectAddresses { get; set; }
    }

    [Table("ObjectAddresses")]
    public class ObjectAddress
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey("Object")]
        [Column("ObjectID")]
        public int ObjectID { get; set; }

        [ForeignKey("Street")]
        [Column("StreetID")]
        public int StreetID { get; set; }

        [Column("HouseNumber")]
        public int HouseNumber { get; set; }

        public virtual Street Street { get; set; }
        public virtual GeneralObject Object { get; set; }
    }

    [Table("ObjectTypes")]
    public class ObjectType
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("ObjectType")]
        public string Title { get; set; }

        public virtual ICollection<GeneralObject> Objects { get; set; }
    }

    [Table("ObjectWorkTimes")]
    public class ObjectWorkTime
    {
        [Key]
        [ForeignKey("Object")]
        [Column("ObjectID")]
        public int ID { get; set; }

        [Column("OpenTime")]
        public DateTime OpenTime { get; set; }

        [Column("CloseTime")]
        public DateTime CloseTime { get; set; }

        public virtual GeneralObject Object { get; set; }
    }

    [Table("ObjectLocations")]
    public class ObjectLocation
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey("ObjectAddress")]
        [Column("ObjectAddressID")]
        public int ObjectAddressID { get; set; }

        [Column("Latitude")]
        public float Latitude { get; set; }

        [Column("Longitude")]
        public float Longitude { get; set; }

        public virtual ObjectAddress ObjectAddress { get; set; }
    }

    [Table("ObjectOptions")]
    public class ObjectOption
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Option")]
        public string Option { get; set; }

        [Column("Position")]
        public byte Position { get; set; }

        [Column("Icon")]
        public string Icon { get; set; }

        public virtual ICollection<GeneralObject> Objects { get; set; }
    }

    [Table("ObjectComments")]
    public class ObjectComment
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey("User")]
        [Column("UserID")]
        public Guid UserID { get; set; }

        [ForeignKey("Object")]
        [Column("ObjectID")]
        public int ObjectID { get; set; }

        [Column("Text")]
        public string Text { get; set; }

        [Column("AddTime")]
        public DateTime AddTime { get; set; }

        public virtual User User { get; set; }
        public virtual GeneralObject Object { get; set; }
    }

    [Table("ObjectStatistics")]
    public class ObjectStatistic
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey("User")]
        [Column("UserID")]
        public Guid UserID { get; set; }

        [ForeignKey("Object")]
        [Column("ObjectID")]
        public int ObjectID { get; set; }

        [Column("LikeCooking")]
        public byte? LikeCooking { get; set; }

        [Column("LikeService")]
        public byte? LikeService { get; set; }

        [Column("LikeInterior")]
        public byte? LikeInterior { get; set; }

        [Column("LikePrice")]
        public byte? LikePrice { get; set; }

        public virtual User User { get; set; }
        public virtual GeneralObject Object { get; set; }
    }

    [Table("Objects")]
    public class GeneralObject
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Caption")]
        public string Caption { get; set; }

        [ForeignKey("Type")]
        [Column("TypeID")]
        public int TypeID { get; set; }

        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Column("Icon")]
        public string Icon { get; set; }

        [Column("IsWork")]
        public bool IsWork { get; set; }

        [Column("Deleted")]
        public bool Deleted { get; set; }

        public virtual ObjectType Type { get; set; }
        public virtual ObjectWorkTime WorkTime { get; set; }
        public virtual ICollection<ObjectAddress> Addresses { get; set; }
        public virtual ICollection<ObjectOption> Options { get; set; }
        public virtual ICollection<ObjectComment> Comments { get; set; }
        public virtual ICollection<ObjectStatistic> Statistics { get; set; }
        public virtual ICollection<ObjectMenuItem> MenuItems { get; set; }
    }

    [Table("FoodTypes")]
    public class FoodType
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        // Поменял имя поля (не компилируется)
        [Column("FoodType")]
        public string Title { get; set; }

        public virtual ICollection<ObjectMenuItem> MenuItems { get; set; } // Внешняя ссылка от ObjectMenuItem
    }

    [Table("ObjectMenu")]
    public class ObjectMenuItem
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey("Object")]
        [Column("ObjectID")]
        public int ObjectID { get; set; }

        [ForeignKey("FoodType")] // Не объявил виртуальное свойство FoodType
        [Column("FoodTypeID")]
        public int FoodTypeID { get; set; } // Поле типа int

        [Column("FoodIcon")]
        public string FoodIcon { get; set; }

        [Column("FoodName")]
        public string FoodName { get; set; }

        [Column("FoodConsist")]
        public string FoodConsist { get; set; }

        [Column("FoodPrice")]
        public int FoodPrice { get; set; }

        [Column("FoodPriceCoins")]
        public Int16 FoodPriceCoins { get; set; }

        // Если указываешь аттрибут ForeignKey, то ты обязательно должен объявить соответствующее виртуальное свойство
        public virtual GeneralObject Object { get; set; }
        // public virtual FoodTypes FoodType { get; set; } - со старым именем класса (немного вводит в замешательство)
        public virtual FoodType FoodType { get; set; }
    }
}