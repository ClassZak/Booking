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
		public DescriptionPathsContainer PathsContainer = new DescriptionPathsContainer();
		public List<HotelInfo> hotelInfos = new List<HotelInfo>();

		public MainWindow()
		{
			InitializeComponent();

			LoadPaths(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data", "Description.JSON"));
			LoadHotelsInfo();
			SetInfoComboBoxList();
			HotelComboBox.SelectedIndex = 0;
			SaveAllBookedRooms();

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
					JsonSerializer.Deserialize<HotelInfo>(streamReader.ReadToEnd());

					if (hotelInfo is null)
						throw new ArgumentException
						($"Ошибка загрузки информации об отеле из файла{PathsContainer.DescriptionPaths[i]}");

					hotelInfos.Add(hotelInfo);

					streamReader.Close();
				}
				catch (FileNotFoundException e)
				{
					MessageBox.Show
					($"Файл{e.FileName} не найден", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
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
		}


		private void NumberOfPeopleTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key.ToString()[0] != 'D' && e.Key.ToString().Length != 2)
				e.Handled = true;
		}

		private void NumberOfPeopleTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
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
					(System.IO.Path.Combine("Data", "Booked hotel rooms", hotelInfos[i].IndexName+".JSON"), hotelInfos[i]);
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
					new BookedRooms(hotelInfo.Name, new List<int>(), hotelInfo.Rooms, hotelInfo.Rooms);

					StreamWriter streamWriter =
					new StreamWriter(new FileStream(path, FileMode.Create,FileAccess.Write));
                    streamWriter.Write(JsonSerializer.Serialize(bookedRooms, typeof(BookedRooms)));

					streamWriter.Close();
				}
			});
		}
	}
}
