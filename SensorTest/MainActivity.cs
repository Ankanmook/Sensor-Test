using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Hardware;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Telephony;
using Android.Net.Wifi;
using Java.Net;
using Java.Lang;
using System.Net;
using Microsoft.WindowsAzure.MobileServices;
using SensorTest;

namespace SensorTest
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon="@drawable/ic_launcher")]
	public class MainActivity : Activity 
	{
		/*
		 * Declaring Class Vairables 
		 */

		//Telephony Manager,wifi,battery manger objects
		TelephonyManager telephonyManager; 
		WifiManager wifiManager;
		BatteryManager batteryManager;

		//Text Views
		private TextView _IpTextView;
		private TextView _deviceIdTextView;
		private TextView _SimSerialNumberTextView;
		private TextView _PhoneNoTextView;
		private TextView _RegisteredTextView;
		private TextView _textTimeStampTextView;
		private TextView _textPercentileTextView;

		string locationInfo;
		string device_ID;
		string device_Info; //Manufactures and model information

		public static MobileServiceClient MobileService;

		/*
		 * Create Method
		 * Fires when the activity is started
		 */
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);
			//setting the main layout Main.xml
			SetContentView(Resource.Layout.Main);

			//intializeSubscriptionToService ();

			//Getting telephony and wifi information from the system service
			telephonyManager = (TelephonyManager)this.GetSystemService(Context.TelephonyService);
			wifiManager= (WifiManager)this.GetSystemService(Service.WifiService); 

			IntentFilter filter = new IntentFilter(Intent.ActionBatteryChanged);


			//Declaring the command ubttons
			var cmd_SensorStatus = FindViewById<Button> (Resource.Id.sensorStatusButton);
			var cmd_Calculate = FindViewById<Button> (Resource.Id.cmdCalculate);

			//Adding text view by id
			_deviceIdTextView = FindViewById<TextView> (Resource.Id.textDeviceID);
			_SimSerialNumberTextView = FindViewById<TextView> (Resource.Id.textSimSerialNumber);
			_PhoneNoTextView = FindViewById<TextView> (Resource.Id.textPhoneNo);
			_IpTextView = FindViewById<TextView> (Resource.Id.textIP);
			_RegisteredTextView =  FindViewById<TextView> (Resource.Id.textRegistered);
			_textTimeStampTextView=  FindViewById<TextView> (Resource.Id.textTimeStamp);
			_textPercentileTextView =  FindViewById<TextView> (Resource.Id.textPercentile);

			//Adding text to the text views from device information
			_deviceIdTextView.Text = telephonyManager.DeviceId.ToString ();
			_SimSerialNumberTextView.Text = telephonyManager.SimSerialNumber.ToString ();
			_PhoneNoTextView.Text = telephonyManager.Line1Number.ToString ();


			device_ID = _deviceIdTextView.Text;
			device_Info = Build.Manufacturer.ToString ()+ " " + Build.Model.ToString ();


			_textTimeStampTextView.Text = DateTime.Now.ToString ();

			locationInfo = telephonyManager.CellLocation.ToString ();

			//Getting localip address
			int ip = wifiManager.ConnectionInfo.IpAddress;
			_IpTextView.Text = string.Format("{0}.{1}.{2}.{3}",(ip & 0xff),(ip >> 8 & 0xff),(ip >> 16 & 0xff),(ip >> 24 & 0xff));

			//Adding even listeners buttons
			cmd_SensorStatus.Click += (object sender, EventArgs e) => {sensorStatusButtonClick();};
			cmd_Calculate.Click += (object sender, EventArgs e) => {calculateButtonClick();};

		}

		/*
		public async void intializeSubscriptionToService(){
		
			MobileService = new MobileServiceClient(
				"https://sentest.azure-mobile.net/",
				"wfCnZkVHVzsoCSTbEJagOpXwaFfQKQ45");

				CurrentPlatform.Init();
				Item item = new Item { Text = "Awesome item" };
				await MobileService.GetTable<Item>().InsertAsync(item);
		}*/

		/*
		* This method deal out with sensor status button click
		* It opens the Sensor Status Screen
		*/
		public void sensorStatusButtonClick(){
			var intent = new Intent(this, typeof(SensorStatusActivity));
			intent.PutExtra("device_Id",device_ID);
			intent.PutExtra("device_Info",device_Info);
			//intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
			StartActivity(intent);
		}

		/*Fist it will check if the device is 
		 * registered to the service or not 
		 * if not registered then it will 
		 * register otherwise it would record
		 * the serviceThis will call the web 
		 * service back to recalculate the details */
		public void calculateButtonClick(){

		}

		
		/*
		 * Gets location wifi address from all the adapters,
		 * ether wifi or if directly converted to internet
		 */
		public string getLocalIpAddress() {
			try {
				for (var en = NetworkInterface.NetworkInterfaces; en.HasMoreElements;) {
					NetworkInterface intf = (NetworkInterface) en.NextElement();
					for (var enumIpAddr = intf.InetAddresses; enumIpAddr.HasMoreElements;) {
						InetAddress inetAddress = (InetAddress) enumIpAddr.NextElement();
						if (!inetAddress.IsLoopbackAddress) {
							//int ip =  inetAddress.GetHashCode();  //String.Format(inetAddress  //Formatter.formatIpAddress(inetAddress.hashCode());

							int ip = inetAddress.GetHashCode();

							string ipStr = string.Format("{0}.{1}.{2}.{3}",(ip & 0xff),(ip >> 8 & 0xff),(ip >> 16 & 0xff),(ip >> 24 & 0xff));

							return ipStr;
						}
					}
				}
			} catch (SocketException ex) {
				//Log.e(TAG, ex.toString());
			} 
			return null;
		}

		/*
		public void onReceive(Context context, Intent intent) {
			bool isPresent = intent.GetBooleanExtra("present", false);
			//Battery Technology
			string technology = intent.GetStringExtra("technology");
			//Battery Plugged Information
			int plugged = intent.GetIntExtra("plugged", -1);
			//Battery Scale
			int scale = intent.GetIntExtra("scale", -1);
			//Battery Health
			int health = intent.GetIntExtra("health", 0);
			//Battery Charging Status
			int status = intent.GetIntExtra("status", 0);
			//Battery charging level
			int rawlevel = intent.GetIntExtra("level", -1);
			int level = 0;
			Bundle bundle = intent.ge;
			Log.i("BatteryLevel", bundle.ToString());
			if (isPresent) {
				if (rawlevel >= 0 && scale > 0) {
					level = (rawlevel * 100) / scale;
				}
				string info = "Battery Level: " + level + "%\n";
				info += ("Technology: " + technology + "\n");
				info += ("Plugged: " +  plugged.ToString() + "\n");
				info += ("Health: " + health.ToString + "\n");
				info += ("Status: " + status.ToString() + "\n");

			} else {
				//setBatteryLevelText("Battery not present!!!");
			}
		}
		*/
	}
		
}



