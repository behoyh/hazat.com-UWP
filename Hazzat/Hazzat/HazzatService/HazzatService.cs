﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using hazzat.com;
using System.Diagnostics;
using System.ServiceModel;
using Xamarin.Forms;
using System.Net.Http;

namespace Hazzat.HazzatService
{
    public class HymnStructNameViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string _season;
        public string Season
        {
            get
            {
                return _season;
            }
            set
            {
                if (value != _season)
                {
                    _season = value;
                    RaisePropertyChanged("Season");
                }
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (value != _content)
                {
                    _content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ByNameMainViewModel
    {
        private const string HazzatServiceEndpoint = "http://hazzat.com/DesktopModules/Hymns/WebService/HazzatWebService.asmx";
        private const int HazzatServiceMaxReceivedMessageSize = 2147483647;
        private const int HazzatServiceMaxBufferPoolSize = 2147483647;
        private const int HazzatServiceMaxBufferSize = 2147483647;

        private BasicHttpBinding HazzatServiceBinding
        {
            get
            {
                return new BasicHttpBinding()
                {
                    MaxReceivedMessageSize = HazzatServiceMaxReceivedMessageSize,
                    MaxBufferSize = HazzatServiceMaxBufferSize,
                    MaxBufferPoolSize = HazzatServiceMaxBufferPoolSize
                };
            }
        }

        /// <summary>
        /// A collection of hazzat.com objects
        /// </summary>
        public SeasonInfo[] Seasons { get; private set; }
        public HymnStructNameViewModel[] Hymns { get; private set; }
        public StructureInfo[] HymnsBySeason { get; private set; }
        public ServiceHymnInfo[] HazzatHymns { get; private set; }
        public ServiceHymnsContentInfo[] HymnContentInfo { get; private set; }
        public ServiceHymnsContentInfo[] HazzatHymnContentInfo { get; private set; }
        public ServiceHymnsContentInfo[] VerticalHazzatHymnContent { get; private set; }

        public TypeInfo[] TypeList { get; private set; }
        public SeasonInfo[] TypeSeasons { get; private set; }

        public TuneInfo[] TuneList { get; private set; }
        public SeasonInfo[] TuneSeasons { get; private set; }

        public void createSeasonsViewModel(bool isDateSpecific)
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetSeasonsCompleted += new EventHandler<GetSeasonsCompletedEventArgs>(client_GetCompleted);
                    client.GetSeasonsAsync(isDateSpecific);
                }
                testConnection.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void client_GetCompleted(object sender, GetSeasonsCompletedEventArgs e)
        {
            foreach (var item in e.Result)
            {
                if (item?.ServiceHymnsCount != null)
                {
                    item.Name = $"{item.Name} ({item.ServiceHymnsCount})";
                }
            }


            Seasons = e.Result;
            MessagingCenter.Send(this, "Done");
        }

        public void createViewModelBySeason(int Season)
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetSeasonServicesCompleted += new EventHandler<GetSeasonServicesCompletedEventArgs>(GetCompletedStructBySeason);
                    client.GetSeasonServicesAsync(Season);
                }
                testConnection.Dispose();

            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void GetCompletedStructBySeason(object sender, GetSeasonServicesCompletedEventArgs e)
        {
            HymnsBySeason = e.Result;
            MessagingCenter.Send(this, "DoneSeason");
        }

        public void FetchServiceHymns(int StructId, Action<object, GetSeasonServiceHymnsCompletedEventArgs> GetCompletedHymnsBySeason)
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetSeasonServiceHymnsCompleted += new EventHandler<GetSeasonServiceHymnsCompletedEventArgs>(GetCompletedHymnsBySeason);
                    client.GetSeasonServiceHymnsAsync(StructId);
                }
                testConnection.Dispose();

            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void GetCompletedHymnsBySeason(object sender, GetSeasonServiceHymnsCompletedEventArgs e)
        {
            HazzatHymns = e.Result;
            MessagingCenter.Send(this, "Done");
        }

        public void CreateHymnTextViewModel(int itemId)
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));

                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);

                    client.GetSeasonServiceHymnTextCompleted += new EventHandler<GetSeasonServiceHymnTextCompletedEventArgs>(client_GetCompletedHymnInfo);
                    client.GetSeasonServiceHymnHazzatCompleted += new EventHandler<GetSeasonServiceHymnHazzatCompletedEventArgs>(client_GetCompletedHymnHazzat);
                    client.GetSeasonServiceHymnVerticalHazzatCompleted += new EventHandler<GetSeasonServiceHymnVerticalHazzatCompletedEventArgs>(client_GetCompletedHymnVerticalHazzat);

                    client.GetSeasonServiceHymnTextAsync(itemId);
                    client.GetSeasonServiceHymnHazzatAsync(itemId);
                    client.GetSeasonServiceHymnVerticalHazzatAsync(itemId);
                }
                testConnection.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void client_GetCompletedHymnInfo(object sender, GetSeasonServiceHymnTextCompletedEventArgs e)
        {
            HymnContentInfo = e.Result;
            MessagingCenter.Send(this, "DoneWithHymnText");
        }

        public void client_GetCompletedHymnHazzat(object sender, GetSeasonServiceHymnHazzatCompletedEventArgs e)
        {
            HazzatHymnContentInfo = e.Result;
            MessagingCenter.Send(this, "DoneWithHazzat");
        }

        private void client_GetCompletedHymnVerticalHazzat(object sender, GetSeasonServiceHymnVerticalHazzatCompletedEventArgs e)
        {
            VerticalHazzatHymnContent = e.Result;
            MessagingCenter.Send(this, "DoneWithVerticalHazzat");
        }

        #region byType
        public void GetHymnsByType()
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetTypeListCompleted += new EventHandler<GetTypeListCompletedEventArgs>(GetCompletedTypeList);
                    client.GetTypeListAsync();
                }
                testConnection.Dispose();

            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void GetCompletedTypeList(object sender, GetTypeListCompletedEventArgs e)
        {
            foreach (var item in e.Result)
            {
                if (item?.ServiceHymnsCount != null)
                {
                    item.Name = $"{item.Name} ({item.ServiceHymnsCount})";
                }
            }

            TypeList = e.Result;
            MessagingCenter.Send(this, "DoneWithTypeList");
        }

        public void GetSeasonsByType(int typeId)
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetSeasonsByTypeIDCompleted += new EventHandler<GetSeasonsByTypeIDCompletedEventArgs>(client_GetSeasonsByTypeID);
                    client.GetSeasonsByTypeIDAsync(typeId);
                }
                testConnection.Dispose();

            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void client_GetSeasonsByTypeID(object sender, GetSeasonsByTypeIDCompletedEventArgs e)
        {
            TypeSeasons = e.Result;
            MessagingCenter.Send(this, "DoneWithSeasonsListByType");
        }

        public void GetServiceHymnListBySeasonIdAndTypeId(int seasonId, int typeId, Action<object, GetServiceHymnListBySeasonIdAndTypeIdCompletedEventArgs> getCompletedHymnsBySeasonAndType)
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetServiceHymnListBySeasonIdAndTypeIdCompleted += new EventHandler<GetServiceHymnListBySeasonIdAndTypeIdCompletedEventArgs>(getCompletedHymnsBySeasonAndType);
                    client.GetServiceHymnListBySeasonIdAndTypeIdAsync(seasonId, typeId);
                }
                testConnection.Dispose();

            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void GetServiceHymnListBySeasonIdAndTuneId(int seasonId, int tuneId, Action<object, GetServiceHymnListBySeasonIdAndTuneIdCompletedEventArgs> getCompletedHymnsBySeasonAndTune)
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetServiceHymnListBySeasonIdAndTuneIdCompleted += new EventHandler<GetServiceHymnListBySeasonIdAndTuneIdCompletedEventArgs>(getCompletedHymnsBySeasonAndTune);
                    client.GetServiceHymnListBySeasonIdAndTuneIdAsync(seasonId, tuneId);
                }
                testConnection.Dispose();

            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region byTune
        public void GetHymnsByTune()
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetTuneListCompleted += new EventHandler<GetTuneListCompletedEventArgs>(GetCompletedTuneList);
                    client.GetTuneListAsync();
                }
                testConnection.Dispose();

            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void GetCompletedTuneList(object sender, GetTuneListCompletedEventArgs e)
        {
            foreach (var item in e.Result)
            {
                if (item?.ServiceHymnsCount != null)
                {
                    item.Name = $"{item.Name} ({item.ServiceHymnsCount})";
                }
            }

            TuneList = e.Result;
            MessagingCenter.Send(this, "DoneWithTuneList");
        }

        public void ByTuneGetSeasons(int tuneId)
        {
            MessagingCenter.Send(this, "Loading");
            try
            {
                HttpClient testConnection = new HttpClient();

                if (!String.IsNullOrWhiteSpace(testConnection.GetAsync("http://hazzat.com").Result.Content.ToString()))
                {
                    HazzatWebServiceSoapClient client = new HazzatWebServiceSoapClient(HazzatServiceBinding, new EndpointAddress(HazzatServiceEndpoint));
                    client.InnerChannel.OperationTimeout = System.TimeSpan.FromSeconds(10);
                    client.InnerChannel.Faulted += new EventHandler(SendConnectionFailure);
                    client.GetSeasonsByTuneIDCompleted += new EventHandler<GetSeasonsByTuneIDCompletedEventArgs>(client_ByTuneGetSeasons);
                    client.GetSeasonsByTuneIDAsync(tuneId);
                }
                testConnection.Dispose();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void client_ByTuneGetSeasons(object sender, GetSeasonsByTuneIDCompletedEventArgs e)
        {
            TuneSeasons = e.Result;
            MessagingCenter.Send(this, "DoneWithSeasonsListByTune");
        }



        #endregion

        private void SendConnectionFailure(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "SEGFAULT");
        }

    }
}
