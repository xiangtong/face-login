
using System;

namespace WeddingPlanner.Models
{
    public class Activity:BaseEntity
    {
        public int Id {get; set;}
        public float Amount {get;set;}
        public DateTime CreatedAt {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
    }
}