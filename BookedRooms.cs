using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking
{
    public class BookedRooms
    {
        public string Name { get; set; }
        public uint RoomCount { get; set; }
        public uint EmptyRooms { get; set; }
        public List<int> Rooms { get; set; }
        

        public BookedRooms(string name, List<int> rooms, uint roomCount,uint emptyRooms)
        {
            RoomCount=roomCount;
            EmptyRooms=emptyRooms;
            Name = name;
            Rooms = rooms;
        }
        public BookedRooms(string name, uint roomCount, uint emptyRooms,params int[] rooms)
        {
            RoomCount = roomCount;
            EmptyRooms = emptyRooms;
            Name = name;
            Rooms = new List<int>(rooms);
        }
    }
}
