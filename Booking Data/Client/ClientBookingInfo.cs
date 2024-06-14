using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Booking
{
	public class ClientBookingInfo
	{
		public Client Client { get; set; }
		public List<BookedRoom> BookedRooms { get; set; }
		public float TotalAmount { get; set; }


		public ClientBookingInfo() { }
		public ClientBookingInfo(Client client, List<BookedRoom> bookedRooms)
		{
			Client = client;
			this.BookedRooms = bookedRooms;

			UpdateTotalAmount();
        }
		public string GetFileName()
		{
			string res = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}.{DateTime.Now.Millisecond} {Client.Name} {Client.Surname} {Client.FatherName}.JSON";

			return res;
		}
		public void UpdateTotalAmount()
		{
            TotalAmount = 0;

            foreach (BookedRoom client1 in BookedRooms)
                TotalAmount += client1.Price;
        }

		public void SaveToFile()
		{
			StreamWriter streamWriter =
			new StreamWriter
			(
				new FileStream
				(
					System.IO.Path.Combine
					(
						Directory.GetCurrentDirectory(),
						"Data",
						"Booked hotel rooms",
						"ClientsRooms",
                        GetFileName()
                    ),
					FileMode.Create,
					FileAccess.Write
				)
			);

			streamWriter.Write(JsonSerializer.Serialize (this));
            streamWriter.Close();
        }
	}
}
