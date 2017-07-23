using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ListPicker.Resources;

using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using GoogleAds;
using System.Net.NetworkInformation;
using Microsoft.Phone.Info;

namespace ListPicker
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        private int versionapp = 2;
        private static string urlIP = "http://mss.c4.vms.com.vn:8081";
        public string urldmtinhsupport = "http://mss.c4.vms.com.vn:8081/RESTfulProject/REST/WebService18/dmtinhsupport";
        public string urldmhuyen = "http://mss.c4.vms.com.vn:8081/RESTfulProject/REST/WebService17/dmhuyen?tinhid=";
        public string urllichmatdienhuyen = "http://mss.c4.vms.com.vn:8081/weblogalarm/lichmatdienhuyen.jsp?huyenid=";
        private string urllog = urlIP + "/weblogalarm/log.jsp";
        private string strsecure = "";
        private string tinhid_select="";
        private string huyenid_select = "";
        private string tinh_select = "";
        private string huyen_select = "";
        private int i = 0;
        private AdView bannerAd;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                tinh_select = IsolatedStorageSettings.ApplicationSettings["tinh"].ToString();
                huyen_select = IsolatedStorageSettings.ApplicationSettings["huyen"].ToString();
                tinhid_select = IsolatedStorageSettings.ApplicationSettings["tinhid"].ToString();
                huyenid_select = IsolatedStorageSettings.ApplicationSettings["huyenid"].ToString();
            }
            catch (Exception ee2)
            {
                tinh_select = "Can Tho";
                huyen_select = "Toan tinh";
                tinhid_select = "cantho";
                huyenid_select = "toantinh";
            }
            if (NetworkInterface.GetIsNetworkAvailable() == true)
            {
                byte[] myDeviceID = (byte[]) DeviceExtendedProperties.GetValue("DeviceUniqueId");
                string DeviceIDAsString = Convert.ToBase64String(myDeviceID);
                log("WP.lichcupdien_v2", DeviceIDAsString);
                getpara();
                get_dmtinh();
            }
            else
            {
                MessageBox.Show("Vui lòng kết nối mạng và thử lại");
                App.Current.Terminate();

            }
           
        }
        private void log(string action, string noidung)
        {
            string str1 = urllog + "?imsi=45201" + "&action=" + action + "&smsdata=" + noidung + "&cell=unknown";
            WebClient webclient = new WebClient();
            webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(logincomplete);
            webclient.DownloadStringAsync(new Uri(str1));
        }
        void logincomplete(object sender, DownloadStringCompletedEventArgs e)
        {
            return;
        }
        private void getpara()
        {
            WebClient webclient = new WebClient();
            webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(getpara_DownloadStringCompleted);
            webclient.DownloadStringAsync(new Uri(urlIP + "/RESTfulProject/REST/apppara/apppara?id=lichcupdien"));
        }
        void getpara_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var jsonString = e.Result;
                var deserialized = JsonConvert.DeserializeObject<List<Para>>(jsonString);
                Para item = new Para();
                foreach (Para json in deserialized)
                {
                    item = json;
                }
                strsecure = item.secure;
                string vs = item.version;
                int currentVs = Int32.Parse(vs);

                if (currentVs > versionapp)
                {
                    // MessageBox.Show("Vui lòng cập nhật phiên bản mới để sử dụng tốt hơn");
                    //Store the Messagebox result in result variable
                    MessageBoxResult result = MessageBox.Show("Cập nhật phiên bản mới để sử dụng tốt hơn", "Thông báo", MessageBoxButton.OKCancel);

                    //check if user clicked on ok
                    /*
                    if (result == MessageBoxResult.OK)
                    {
                        //Add the task you wish to perform here            
                        WebBrowserTask namewhatevz = new WebBrowserTask();
                        namewhatevz.Uri = new Uri(urlappWP, UriKind.Absolute);
                        namewhatevz.Show();
                    }
                     * */
                }
            }
            catch (Exception e1)
            {
                loi_exit();
            }

        }
        private void loi_thongbao()
       {
           MessageBoxResult result = MessageBox.Show("Có lỗi xảy ra khi kết nối server", "Thông báo", MessageBoxButton.OK);
       }
        private void loi_exit()
        {
            MessageBoxResult result = MessageBox.Show("Không thể kết nối server", "Thông báo", MessageBoxButton.OK);
            Application.Current.Terminate();

        }
        private void get_dmtinh()
        {
             List<ObjTinh> tinhList = new List<ObjTinh>();
            WebClient webclient = new WebClient();
            webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(get_dmtinh_DownloadStringCompleted);
            webclient.DownloadStringAsync(new Uri(urldmtinhsupport));
        }
        void get_dmtinh_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try { 
                List<ObjTinh> tinhList = new List<ObjTinh>();
            
                ObjTinh chontinh = new ObjTinh();
                chontinh.tinh = tinh_select; chontinh.tinhid = tinhid_select;
                tinhList.Add(chontinh);
                var jsonString = e.Result;
            
                var deserialized = JsonConvert.DeserializeObject<List<ObjTinh>>(jsonString);
                foreach (ObjTinh item in deserialized)
                {
                    tinhList.Add(item);
                }
                this.listPickerTinh.ItemsSource = tinhList;
            }
            catch (Exception e1)
            {
                loi_exit();
            }

        }
        private void get_dmhuyen(string tinhid)
        {
            WebClient webclient = new WebClient();
            webclient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(get_dmhuyen_DownloadStringCompleted);
            string url2 = urldmhuyen + tinhid;
            webclient.DownloadStringAsync(new Uri(url2));
        }
        void get_dmhuyen_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try { 
                List<ObjHuyen> huyenList = new List<ObjHuyen>();
                ObjHuyen chonhuyen = new ObjHuyen();
                chonhuyen.huyen = huyen_select; chonhuyen.huyenid = huyenid_select;
                huyenList.Add(chonhuyen);

                var jsonString = e.Result;
                ObjHuyen toan= new ObjHuyen();
                toan.huyen="Toan tinh"; toan.huyenid="toantinh";           
            
                var deserialized = JsonConvert.DeserializeObject<List<ObjHuyen>>(jsonString);
                huyenList.Add(toan);
                foreach (ObjHuyen item in deserialized)
                {
                    huyenList.Add(item);
                }
                this.listPickerHuyen.ItemsSource = huyenList;
                i++;

                string site = urllichmatdienhuyen + huyenid_select + "&tinhid=" + tinhid_select+"&sc="+strsecure+strsecure;
                webBrowser1.Navigate(new Uri(site, UriKind.Absolute));
                //System.Diagnostics.Debug.WriteLine("Time {0}", DateTime.Now + site);
            }
            catch (Exception e1)
            {
                loi_exit();
            }
        }
       
        public class Para
        {
            public string version { get; set; }
            public string secure { get; set; }
          
        }
        public class ObjTinh
        {
            public string tinhid { get; set; }
            public string tinh { get; set; }
        }
        public class ObjHuyen
        {
            public string huyenid { get; set; }
            public string huyen { get; set; }
        }
     
      
        private void chonhuyen(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ObjHuyen chonhuyen = ((sender as Telerik.Windows.Controls.RadListPicker).SelectedItem as ObjHuyen);
                huyenid_select = chonhuyen.huyenid;
                huyen_select = chonhuyen.huyen;

                var picker = sender as Telerik.Windows.Controls.RadListPicker;
                //MessageBox.Show("chon:" + lbi.gethuyen());
                string site = urllichmatdienhuyen + huyenid_select + "&tinhid=" + tinhid_select + "&sc=" + strsecure + strsecure;
                webBrowser1.Navigate(new Uri(site, UriKind.Absolute));
                //Console.WriteLine(site);
                System.Diagnostics.Debug.WriteLine("Time {0}", DateTime.Now+ site);
            }
            catch (Exception ee1)
            { }
            if (i >= 1)
            {
                IsolatedStorageSettings.ApplicationSettings.Clear();
                IsolatedStorageSettings.ApplicationSettings.Add("huyenid", huyenid_select);
                IsolatedStorageSettings.ApplicationSettings.Add("tinhid", tinhid_select);
                IsolatedStorageSettings.ApplicationSettings.Add("huyen", huyen_select);
                IsolatedStorageSettings.ApplicationSettings.Add("tinh", tinh_select);
                IsolatedStorageSettings.ApplicationSettings.Save();
                System.Diagnostics.Debug.WriteLine("Time {0}", DateTime.Now + tinhid_select + "-" + huyenid_select);
            }

        }

        private void chontinh(object sender, SelectionChangedEventArgs e)
        {
            ObjTinh chontinh = ((sender as Telerik.Windows.Controls.RadListPicker).SelectedItem as ObjTinh);
            var picker = sender as Telerik.Windows.Controls.RadListPicker;
            //MessageBox.Show("chon:" + chontinh.tinh);
            tinhid_select = chontinh.tinhid;
            tinh_select = chontinh.tinh;
            get_dmhuyen(tinhid_select);
         
        }

        private void OnAdReceived(object sender, AdEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Received ad successfully");
        }

        private void OnFailedToReceiveAd(object sender, AdErrorEventArgs errorCode)
        {
            System.Diagnostics.Debug.WriteLine("Failed to receive ad with error " + errorCode.ErrorCode.ToString());
            adview.Visibility=Visibility.Collapsed;
        }
    }
}