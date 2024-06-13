using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking
{
    public class BookedRoom
    {
        public string HotelName { get; set; }
        public int Number { get; set;}
        public DateTime DateTimeIn { get; set; }
        public DateTime DateTimeOut { get; set; }
        public float Price { get; set; }
        public BookedRoom() { }
        public BookedRoom(string hotelName, int number, DateTime dateTimeIn, DateTime dateTimeOut,float price)
        {
            HotelName = hotelName;
            Number = number;
            DateTimeIn = dateTimeIn;
            DateTimeOut = dateTimeOut;
            Price = price;
        }
        public BookedRoom(BookedRoom room)
        {
            HotelName =room.HotelName;
            Number = room.Number;
            DateTimeIn = room.DateTimeIn;
            DateTimeOut = room.DateTimeOut;
            Price = room.Price;
        }
    }
}
