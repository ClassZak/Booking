using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking
{
	public class HotelInfo
	{
		public string IndexName { get; set; }
        public string Name { get; set; }
		public string Description { get; set; }
		public uint Rooms { get; set; }
		public float Price { get; set; }
		public string ImagePath { get; set; }


		public HotelInfo()
		{

		}
		public HotelInfo(string name, string description, uint rooms, float price, string imagePath)
		{
			this.Name = name;
			this.Description = description;
			this.Rooms = rooms;
			this.Price = price;
			this.ImagePath = imagePath;
		}
	}
}
