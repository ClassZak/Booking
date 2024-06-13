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


			TotalAmount= 0;

			foreach(BookedRoom client1 in bookedRooms)
				TotalAmount += client1.Price;
		}
		public string GetFileName()
		{
			return $"{DateTime.Now} {Client.Name} {Client.Surname} {Client.FatherName}";
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
                        GetFileName()+".JSON"
                    ),
					FileMode.Create,
					FileAccess.Write
				)
			);

			streamWriter.Write(JsonSerializer.Serialize (BookedRooms));
            streamWriter.Close();
        }
	}
}
