using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.IO;

namespace Booking
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        public ClientBookingInfo ClientBookingInfo = new ClientBookingInfo();
        public DescriptionPathsContainer PathsContainer = new DescriptionPathsContainer();
		public List<HotelInfo> hotelInfos = new List<HotelInfo>();
		bool bookingStarted = false;

		public MainWindow()
		{
			InitializeComponent();

            NumberOfPeopleTextBox.Minimum= 1;

            LoadPaths(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Description.JSON"));
			LoadHotelsInfo();
			SetInfoComboBoxList();
			HotelComboBox.SelectedIndex = 0;
			//SaveAllBookedRooms();
			UpdateEmptyRoomsCount();
        }



		void LoadPaths(string path)
		{
			try
			{
				StreamReader streamReader = new StreamReader
				(new FileStream(path, FileMode.Open, FileAccess.Read));

				PathsContainer =
				JsonSerializer.Deserialize<DescriptionPathsContainer>(streamReader.ReadToEnd());

				streamReader.Close();
			}
			catch (FileNotFoundException)
			{
				MessageBox.Show
				(
					"Отсутствуют данные для работы приложения",
					"Ошибка запуска",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				Close();
			}
		}
		void LoadHotelsInfo()
		{
			if (PathsContainer.DescriptionPaths.Count == 0)
			{
				MessageBox.Show
				(
					"Ошибка загрузки информации об отелях",
					"Ошибка загрузки",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
				Close();
				return;
			}

			for (int i = 0; i < PathsContainer.DescriptionPaths.Count; ++i)
			{
				try
				{
					StreamReader streamReader =
					new StreamReader
					(new FileStream(PathsContainer.DescriptionPaths[i], FileMode.Open, FileAccess.Read));

					HotelInfo hotelInfo =
					JsonSerializer.Deserialize<HotelInfo>(streamReader.ReadToEnd()) ?? throw new ArgumentException
						(
							$"Ошибка загрузки информации об отеле из файла{PathsContainer.DescriptionPaths[i]}"
						);
                    hotelInfos.Add(hotelInfo);

					streamReader.Close();
				}
				catch (FileNotFoundException e)
				{
					MessageBox.Show
					(
						$"Файл{e.FileName} не найден",
						"Ошибка загрузки", MessageBoxButton.OK,
						MessageBoxImage.Error
					);
				}
			}
		}
		void SetInfoComboBoxList()
		{
			HotelComboBox.Items.Clear();

			for (int i = 0; i < hotelInfos.Count; ++i)
				HotelComboBox.Items.Add(hotelInfos[i].Name);
		}
		void LoadHotelImage(int infoNumber)
		{
			try
			{
				HotelImage.Source = new BitmapImage
				(new Uri(System.IO.Path.Combine(Directory.GetCurrentDirectory(), hotelInfos[infoNumber].ImagePath)));
			}
			catch (UriFormatException)
			{
				MessageBox.Show
				($"Неверный формат пути файла:\n{hotelInfos[infoNumber].ImagePath}", "Ошибка загрузки");
				Close();
				return;
			}
			catch (FileNotFoundException e)
			{
				MessageBox.Show
				($"Файл{e.FileName} не найден", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void HotelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadHotelImage(HotelComboBox.SelectedIndex);
			HotelDescription.Text = hotelInfos[HotelComboBox.SelectedIndex].Description;
			PriceTextBox.Text = hotelInfos[HotelComboBox.SelectedIndex].Price.ToString();
			TotalAmountTextBox.Text =
			(hotelInfos[HotelComboBox.SelectedIndex].Price * NumberOfPeopleTextBox.Value).ToString();
			RoomCountTexBox.Text = hotelInfos[HotelComboBox.SelectedIndex].Rooms.ToString();
            EmptyRoomsTextBox.Text = hotelInfos[HotelComboBox.SelectedIndex].Rooms.ToString();

			UpdateEmptyRoomsCountAsync();
        }


		private void NumberOfPeopleTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key.ToString()[0] != 'D' && e.Key.ToString().Length != 2)
				e.Handled = true;
		}

		private void NumberOfPeopleTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (!IsInitialized)
				return;
			try
			{
				TotalAmountTextBox.Text =
				(this.hotelInfos[HotelComboBox.SelectedIndex].Price * NumberOfPeopleTextBox.Value).ToString();
			}
			catch
			{
			}
		}
		private async void SaveAllBookedRooms()
		{
			await Task.Run(() =>
			{
				for (int i = 0; i < hotelInfos.Count; ++i)
				{
					SaveBookedRooms
					(System.IO.Path.Combine(Directory.GetCurrentDirectory(),"Data", "Booked hotel rooms", hotelInfos[i].IndexName+".JSON"), hotelInfos[i]);
                }
			});
		}
		private async void SaveBookedRooms(string path, HotelInfo hotelInfo)
		{
			await Task.Run(() =>
			{
				if (!File.Exists(path))
				{
					BookedRooms bookedRooms =
					new BookedRooms(hotelInfo.Name, new List<BookedRoom>(), hotelInfo.Rooms, hotelInfo.Rooms);

					StreamWriter streamWriter =
					new StreamWriter(new FileStream(path, FileMode.Create,FileAccess.Write));
                    streamWriter.Write(JsonSerializer.Serialize(bookedRooms, typeof(BookedRooms)));

					streamWriter.Close();
				}
			});
		}


		private uint CheckEmptyRooms(string hotelName,DateTime? tIn,DateTime? tOut)
		{
			uint res = 0;
			if (tIn is null || tOut is null)
				return 0;

			int lastNumber = 0;
			Dispatcher.Invoke(() =>
			{
				lastNumber = (int)hotelInfos[HotelComboBox.SelectedIndex].Rooms + 1;
            });
            for
			(
				int i= 1;
				i != lastNumber;
				++i
			)
            {
				bool addItem = true;
                foreach
                (
                    string file in
                    Directory.GetFiles
                    (
                        System.IO.Path.Combine
                        (
                            Directory.GetCurrentDirectory(),
                            "Data",
                            "Booked hotel rooms",
                            "ClientsRooms"
                        )
                    )
                )
                {
                    StreamReader streamReader = new StreamReader
                    (
                        new FileStream
                        (
                            file,
                            FileMode.Open,
                            FileAccess.Read
                        )
                    );
                    ClientBookingInfo bookedRoom =
					JsonSerializer.Deserialize<ClientBookingInfo>(streamReader.ReadToEnd());
                    streamReader.Close();


					if
					(
						(!(bookedRoom.BookedRooms.Find
						(
							x =>
							x.HotelName == hotelName &&
							x.Number == i &&
							(
								tIn <= x.DateTimeIn &&
								x.DateTimeIn <= tOut
							)
						) is null))
					)
					{
						addItem = false;
						break;
					}
                }
				if (addItem)
					++res;
				addItem = true;
            }
			return res;
        }

		private async void Booking_Click(object sender, RoutedEventArgs e)
		{
			if(bookingStarted)
			{
				MessageBox.Show("Процесс бронирования номеров запущен", "Операция выполняется", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}
			bookingStarted = true;


            if (GuestNameBox.Text is null || GuestNameBox.Text == "")
			{
				MessageBox.Show("Пожалуйста введите имя.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
				bookingStarted = false;
                return;
			}


			if (GuestSurnameBox.Text is null || GuestSurnameBox.Text == "")
			{
				MessageBox.Show("Пожалуйста введите фамилию.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                bookingStarted = false;
                return;
			}

			if (FatherName.Text is null || FatherName.Text == "")
			{
				MessageBox.Show("Пожалуйста введите отчество.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                bookingStarted = false;
                return;
			}


			if (CheckInDatePicker.Text == "" || CheckInDatePicker.SelectedDate < DateTime.Today.Date)
			{
				MessageBox.Show("Пожалуйста введите дату въезда.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                bookingStarted = false;
                return;
			}

			if (CheckOutDatePicker.Text == "")
			{
				MessageBox.Show("Пожалуйста введите дату выезда.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                bookingStarted = false;
                return;
			}
			if (NumberOfPeopleTextBox.Text == "" || NumberOfPeopleTextBox.Text is null)
			{
				MessageBox.Show("Пожалуйста введите количество людей.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                bookingStarted = false;
                return;
			}
            if (CheckOutDatePicker.Text == "" || CheckOutDatePicker.SelectedDate < CheckInDatePicker.SelectedDate)
            {
                MessageBox.Show("Пожалуйста введите корректную дату выезда.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                bookingStarted = false;
                return;
            }

            

			
			await Task.Run(() =>
			{
				{
					try
					{
						UpdateEmptyRoomsCount();


						uint emptyPlaces=0, _toSell=0;
						Dispatcher.Invoke(() =>
						{
							emptyPlaces = uint.Parse(EmptyRoomsTextBox.Text);
							_toSell = uint.Parse(NumberOfPeopleTextBox.Text);

                        });

						if (emptyPlaces < _toSell)
						{
							MessageBox.Show
							(
								"Неверное число забронированнх номеров",
								"Ошибка ввода",
								MessageBoxButton.OK,
								MessageBoxImage.Warning
							);
							return;
						}
					}
					catch
					{
					}
				}

				ClientBookingInfo.Client = new Client();

				Dispatcher.Invoke(() =>
				{
					ClientBookingInfo.Client.Name = GuestNameBox.Text;
					ClientBookingInfo.Client.Surname = GuestSurnameBox.Text;
					ClientBookingInfo.Client.FatherName = FatherName.Text;

                });
				ClientBookingInfo.BookedRooms = new List<BookedRoom>();

				string IndexName="";
				DateTime dateTimeIn=DateTime.Now;
				DateTime dateTimeOut = DateTime.Now;
				float Price=0.0f;
				Dispatcher.Invoke(() =>
				{
					IndexName = hotelInfos[HotelComboBox.SelectedIndex].IndexName;
					dateTimeIn = (DateTime)CheckInDatePicker.SelectedDate;
					dateTimeOut = (DateTime)CheckOutDatePicker.SelectedDate;
					Price = hotelInfos[HotelComboBox.SelectedIndex].Price;
                });

                BookedRoom bookedRoom1 = new BookedRoom
				(
                    IndexName,
					1,
                    dateTimeIn,
                    dateTimeOut,
                    Price
                );



                uint lastNumber=0, toSell = 0;
				Dispatcher.Invoke(() =>
				{
                    lastNumber=hotelInfos[HotelComboBox.SelectedIndex].Rooms + 1;
					toSell = uint.Parse(NumberOfPeopleTextBox.Text);
				});

				int sellI = 1;
                for
				(
					bookedRoom1.Number = 1;
					bookedRoom1.Number != lastNumber && sellI != toSell+1;
					++bookedRoom1.Number
				)
				{
					bool emptyRoom = true;

					foreach
					(
						string file in
						Directory.GetFiles
						(
							System.IO.Path.Combine
							(
								Directory.GetCurrentDirectory(),
								"Data",
								"Booked hotel rooms",
								"ClientsRooms"
							)
						)
					)
					{
						StreamReader streamReader = new StreamReader
						(
							new FileStream
							(
								file,
								FileMode.Open,
								FileAccess.Read
							)
						);
                        ClientBookingInfo bookedRoom = JsonSerializer.Deserialize<ClientBookingInfo>(streamReader.ReadToEnd());
						streamReader.Close();


						if
						(
							!(bookedRoom.BookedRooms.Find
							(
								x=>x.Number== bookedRoom1.Number &&
								x.HotelName == bookedRoom1.HotelName
								&& 
								(
									x.DateTimeIn<= bookedRoom1.DateTimeIn &&
									x.DateTimeIn<= bookedRoom1.DateTimeOut
								)
							) is null)
						)
						{
                            emptyRoom = false;
							break;
						}
                    }
					if(emptyRoom)
					{
						ClientBookingInfo.BookedRooms.Add(new BookedRoom(bookedRoom1));
						++sellI;
					}
                }
				ClientBookingInfo.UpdateTotalAmount();

                ClientBookingInfo.SaveToFile();

				UpdateEmptyRoomsCount();

                Dispatcher.Invoke(() =>
				{
					MessageBox.Show
					(
						"Ваши данные были записаны.\n"+
						$"Номера в отель \"{hotelInfos[HotelComboBox.SelectedIndex].Name}\" забронированны\n"+
						$"К оплате:{ClientBookingInfo.TotalAmount}"
                        ,
						"Бронирование",
						MessageBoxButton.OK,
						MessageBoxImage.Information
					);

					bookingStarted = false;
                });
			});
        }

        private void CheckOutDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEmptyRoomsCountAsync();
        }

        private void CheckInDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEmptyRoomsCountAsync();
        }

		private async void UpdateEmptyRoomsCountAsync()
		{
            await Task.Run(() =>
            {
                DateTime? tIn = null;
                DateTime? tOut = null;
                string file = "";

                Dispatcher.Invoke(() =>
                {
                    tIn = CheckInDatePicker.SelectedDate;
                    tOut = CheckOutDatePicker.SelectedDate;
                    file = hotelInfos[HotelComboBox.SelectedIndex].IndexName;
                });


                uint res = CheckEmptyRooms(file, tIn, tOut);

                Dispatcher.Invoke(() =>
                {
                    EmptyRoomsTextBox.Text = res.ToString();
                    NumberOfPeopleTextBox.Maximum = (int?)res;
                });
            });
        }
		private void UpdateEmptyRoomsCount()
		{
            DateTime? tIn = null;
            DateTime? tOut = null;
            string file = "";

            Dispatcher.Invoke(() =>
            {
                tIn = CheckInDatePicker.SelectedDate;
                tOut = CheckOutDatePicker.SelectedDate;
                file = hotelInfos[HotelComboBox.SelectedIndex].IndexName;
            });


            uint res = CheckEmptyRooms(file, tIn, tOut);

            Dispatcher.Invoke(() =>
            {
                EmptyRoomsTextBox.Text = res.ToString();
				NumberOfPeopleTextBox.Maximum = (int?)res;
            });
        }
    }
}
