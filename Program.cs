
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using Microsoft.Bond;
using Microsoft.ObjectStore;
using Microsoft.Search.ObjectStore;
using Microsoft.Bing.MultimediaRepository;

namespace StorePad
{
	class Program
	{
		public const string EnvironmentEndpoint = "objectstoremulti.prod.co.binginternal.com:83/sds";
		public const string NamespaceName = "Multimedia";
		public const string TableName = "PageData";

		public const string columnName = "BasicFeatures";
		public const string featureName1 = "PageTitle"; // subkey
		public const string featureName2 = "DomainKey"; // subkey
		public const string featureName3 = "Host"; // subkey
		public const string featureName4 = "VideoData"; // subkey

		public static DateTime s_baseTime = new DateTime(1970, 1, 1);

		public static void Main(string[] args)
		{

			File.WriteAllLines(@"C:\Users\yirshen.REDMOND\Downloads\772527-1.txt", File.ReadAllLines(@"C:\Users\yirshen.REDMOND\Downloads\772527.txt").Select(x => x + "\t" + ReadTitle(SimpleKey.GetUrlBase64Key(x))));


			//TestReadAndWrite();

			//TestRead(args[0]);

			////Console.ReadLine();

			////string fromKey = "/TREaoVmQ7AQtg";
			////string toKey = "/awtGesmiJ538g";

			////Multimedia.StringFeatureValue videoData = null;

			////using (var client = Client.Builder<Multimedia.PageDataKey, Multimedia.PageDataValue>(environment: EnvironmentEndpoint,
			////									  osNamespace: NamespaceName,
			////									  osTable: TableName,
			////									  timeout: new TimeSpan(0, 0, 0, 500),
			////									  maxRetries: 1).Create())
			////{
			////	var key = new Multimedia.PageDataKey
			////	{
			////		PageKey = fromKey
			////	};

			////	var keys = new List<Multimedia.PageDataKey> { key };

			////	List<ColumnLocation> locations = new List<ColumnLocation>();

			////	locations.Add(new ColumnLocation(columnName, featureName1));
			////	locations.Add(new ColumnLocation(columnName, featureName2));
			////	locations.Add(new ColumnLocation(columnName, featureName3));
			////	locations.Add(new ColumnLocation(columnName, featureName4));

			////	var task = client.ColumnReadFromPrimary(keys, locations).WithDebugInfoEnabled().SendAsyncWithDebugFullResponse();

			////	try
			////	{
			////		task.Wait();
			////	}
			////	catch (Exception ex)
			////	{
			////		Console.WriteLine("Read() - Exception: {0}, {1}", ex.Message, ex.StackTrace);
			////	}

			////	var results = task.Result;

			////	Console.WriteLine("results.SubResponses.Count = " + results.SubResponses.Count()); // the count should equal to number of keys

			////	if (results.SubResponses.Count() > 0)
			////	{

			////		Multimedia.StringFeatureValue pageTitleFeature;
			////		Multimedia.StringFeatureValue domainFeature;
			////		Multimedia.StringFeatureValue hostFeature;
			////		Multimedia.StringFeatureValue videoDataFeature;

			////		ObjectStoreWireProtocol.OSColumnOperationResultType res;

			////		res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[0].ColumnName, locations[0].SubKey, out pageTitleFeature);
			////		res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[1].ColumnName, locations[1].SubKey, out domainFeature);
			////		res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[2].ColumnName, locations[2].SubKey, out hostFeature);
			////		res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[2].ColumnName, locations[3].SubKey, out videoDataFeature);

			////		Console.WriteLine($"pageTitle : {pageTitleFeature.Value}");
			////		Console.WriteLine($"domainKey : {domainFeature.Value}");

			////		videoData = videoDataFeature;
			////	}
			////}

			////using (var client = Client.Builder<Multimedia.PageDataKey, Multimedia.PageDataValue>(environment: EnvironmentEndpoint,
			////									  osNamespace: NamespaceName,
			////									  osTable: TableName,
			////									  timeout: new TimeSpan(0, 0, 0, 500),
			////									  maxRetries: 1).Create())
			////{
			////	var key = new Multimedia.PageDataKey
			////	{
			////		PageKey = toKey
			////	};

			////	var columnrecord = client.CreateColumnRecord(key);

			////	columnrecord.SetColumnValue(columnName, featureName4, videoData);

			////	var recordsToWrite = new List<IColumnRecord<Multimedia.PageDataKey>>() { columnrecord };

			////	var task = client.ColumnWrite(recordsToWrite).SendAsync();
			////	try
			////	{
			////		task.Wait();
			////	}
			////	catch (Exception ex)
			////	{
			////		Console.WriteLine("ColumnWrite() - Exception: {0}, {1}", ex.Message, ex.StackTrace);
			////	}

			////	List<ObjectStoreWireProtocol.ObjectStoreQueryStatus> results = task.Result;

			////	foreach (ObjectStoreWireProtocol.ObjectStoreQueryStatus status in results)
			////	{
			////		Console.WriteLine("status = " + status.ToString());
			////	}
			////}

			////Read(toKey);

		}

		public static void TestRead(string key)
		{
			string pageKey = key;

			//Console.WriteLine("pageKey = " + pageKey);

			string afterUpdate = Read(pageKey);

			Console.WriteLine();

			////Console.WriteLine("afterUpdate = " + afterUpdate);
		}

		public static void TestReadAndWrite()
		{
			string pageKey = "++5SYPCzHmMu1w";
			string pageTitle = "Watch Every Witch Way Season 4 Free on Fmovies"; // free --> Free
			string domain = "ac.fmovies."; // no change
			UInt32 updateTime = 1527645000;  // 1527642180 --> 1527645000

			Console.WriteLine("pageKey = " + pageKey);

			string beforeUpdate = Read(pageKey);

			Console.WriteLine("beforeUpdate = " + beforeUpdate);

			Console.WriteLine();

			Write(pageKey, pageTitle, domain, updateTime);

			string afterUpdate = Read(pageKey);

			Console.WriteLine();

			Console.WriteLine("afterUpdate = " + afterUpdate);
		}

		public static void TestDelete()
		{
			string pageKey = "++5SYPCzHmMu1w";

			Console.WriteLine("pageKey = " + pageKey);

			string beforeDelete = Read(pageKey);

			Console.WriteLine("beforeDelete = " + beforeDelete);

			Console.WriteLine();

			Delete(pageKey);

			string afterDelete = Read(pageKey);

			Console.WriteLine();

			Console.WriteLine("afterDelete = " + afterDelete);
		}

		public static string BondToJson(IBondSerializable obj)
		{
			var jsonStream = new MemStream();
			obj.Write(new JsonPrettyWriter(jsonStream));
			jsonStream.Position = 0;
			return new StreamReader(jsonStream).ReadToEnd();
		}

		public static String ReadTitle(string pageKey)
		{
			using (var client = Client.Builder<Multimedia.PageDataKey, Multimedia.PageDataValue>(environment: EnvironmentEndpoint,
												  osNamespace: NamespaceName,
												  osTable: TableName,
												  timeout: new TimeSpan(0, 0, 0, 500),
												  maxRetries: 1).Create())
			{
				var key = new Multimedia.PageDataKey
				{
					PageKey = pageKey
				};

				var keys = new List<Multimedia.PageDataKey> { key };

				List<ColumnLocation> locations = new List<ColumnLocation>();

				locations.Add(new ColumnLocation(columnName, featureName1));
				locations.Add(new ColumnLocation(columnName, featureName2));
				locations.Add(new ColumnLocation(columnName, featureName3));
				locations.Add(new ColumnLocation(columnName, featureName4));

				var task = client.ColumnReadFromPrimary(keys, locations).WithDebugInfoEnabled().SendAsyncWithDebugFullResponse();

				try
				{
					task.Wait();
				}
				catch (Exception ex)
				{
					//Console.WriteLine("Read() - Exception: {0}, {1}", ex.Message, ex.StackTrace);
				}

				var results = task.Result;

				//Console.WriteLine("results.SubResponses.Count = " + results.SubResponses.Count()); // the count should equal to number of keys

				if (results.SubResponses.Count() > 0)
				{

					Multimedia.StringFeatureValue pageTitleFeature;
					Multimedia.StringFeatureValue domainFeature;
					Multimedia.StringFeatureValue hostFeature;
					Multimedia.StringFeatureValue videoDataFeature;

					ObjectStoreWireProtocol.OSColumnOperationResultType res;

					res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[0].ColumnName, locations[0].SubKey, out pageTitleFeature);
					res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[1].ColumnName, locations[1].SubKey, out domainFeature);
					res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[2].ColumnName, locations[2].SubKey, out hostFeature);
					res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[2].ColumnName, locations[3].SubKey, out videoDataFeature);

					if (pageTitleFeature != null)
                    {
						//Console.WriteLine($"pageTitle : {pageTitleFeature.Value}");
						return pageTitleFeature.Value;
					}
						
					if (domainFeature != null)
						//Console.WriteLine($"domainKey : {domainFeature.Value}");

					if (hostFeature != null)
						//Console.WriteLine($"host : {hostFeature.Value}");
					if (videoDataFeature != null)
						//Console.WriteLine($"videoData : {videoDataFeature.Value}");

					////if (pageTitleFeature != null && domainFeature != null)
					////{
					////	return $"pageTitle : {pageTitleFeature.Value} \t domainKey : {domainFeature.Value}";
					////}
					////else
					////{
					////	return "pageTitleFeature or domainFeature is null";
					////}
					return string.Empty;
				}
				else
				{
					return string.Empty;
				}

				return string.Empty;
			}
		}

		public static String Read(string pageKey) 
		{
			using (var client = Client.Builder<Multimedia.PageDataKey, Multimedia.PageDataValue>(environment: EnvironmentEndpoint, 
												  osNamespace: NamespaceName, 
												  osTable: TableName, 
												  timeout: new TimeSpan(0, 0, 0, 500), 
												  maxRetries: 1).Create())
			{
				var key = new Multimedia.PageDataKey
				{
					PageKey = pageKey
				};

				var keys = new List<Multimedia.PageDataKey> { key };

				List<ColumnLocation> locations = new List<ColumnLocation>();

				locations.Add(new ColumnLocation(columnName, featureName1));
				locations.Add(new ColumnLocation(columnName, featureName2));
				locations.Add(new ColumnLocation(columnName, featureName3));
				locations.Add(new ColumnLocation(columnName, featureName4));

				var task = client.ColumnReadFromPrimary(keys, locations).WithDebugInfoEnabled().SendAsyncWithDebugFullResponse();

				try
				{
					task.Wait();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Read() - Exception: {0}, {1}", ex.Message, ex.StackTrace);
				}

				var results = task.Result;

				Console.WriteLine("results.SubResponses.Count = " + results.SubResponses.Count()); // the count should equal to number of keys

				if (results.SubResponses.Count() > 0)
				{

					Multimedia.StringFeatureValue pageTitleFeature;
					Multimedia.StringFeatureValue domainFeature;
					Multimedia.StringFeatureValue hostFeature;
					Multimedia.StringFeatureValue videoDataFeature;

					ObjectStoreWireProtocol.OSColumnOperationResultType res;

					res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[0].ColumnName, locations[0].SubKey, out pageTitleFeature);
					res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[1].ColumnName, locations[1].SubKey, out domainFeature);
					res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[2].ColumnName, locations[2].SubKey, out hostFeature);
					res = results.SubResponses.First().ColumnRecord.GetColumnValue(locations[2].ColumnName, locations[3].SubKey, out videoDataFeature);

					if (pageTitleFeature != null)
						Console.WriteLine($"pageTitle : {pageTitleFeature.Value}");
					if (domainFeature != null)
						Console.WriteLine($"domainKey : {domainFeature.Value}");

					if (hostFeature != null)
						Console.WriteLine($"host : {hostFeature.Value}");
					if (videoDataFeature != null)
						Console.WriteLine($"videoData : {videoDataFeature.Value}");

					////if (pageTitleFeature != null && domainFeature != null)
					////{
					////	return $"pageTitle : {pageTitleFeature.Value} \t domainKey : {domainFeature.Value}";
					////}
					////else
					////{
					////	return "pageTitleFeature or domainFeature is null";
					////}
					return "record found for pageKey = " + pageKey;
				}
				else
				{
					return "no record found for pageKey = " + pageKey;
				}
			}
		}
		
		public static void Write(string pageKey, string pageTitle, string domain, UInt32 updateTime) 
		{
			using (var client = Client.Builder<Multimedia.PageDataKey, Multimedia.PageDataValue>(environment: EnvironmentEndpoint, 
												  osNamespace: NamespaceName, 
												  osTable: TableName, 
												  timeout: new TimeSpan(0, 0, 0, 500), 
												  maxRetries: 1).Create())
			{
				var key = new Multimedia.PageDataKey
				{
					PageKey = pageKey
				};                

				var featureValue1 = new Multimedia.StringFeatureValue
				{
					Value = pageTitle,
					Timestamp = updateTime
				};

				var featureValue2 = new Multimedia.StringFeatureValue
				{
					Value = domain,
					Timestamp = updateTime
				};

				var columnrecord = client.CreateColumnRecord(key);

				columnrecord.SetColumnValue(columnName, featureName1, featureValue1);
				columnrecord.SetColumnValue(columnName, featureName2, featureValue2);

				var recordsToWrite = new List<IColumnRecord<Multimedia.PageDataKey>>() { columnrecord };

				var task = client.ColumnWrite(recordsToWrite).SendAsync();
				try
				{
					task.Wait();
				}
				catch (Exception ex)
				{
					Console.WriteLine("ColumnWrite() - Exception: {0}, {1}", ex.Message, ex.StackTrace);
				}

				List<ObjectStoreWireProtocol.ObjectStoreQueryStatus> results = task.Result;

				foreach(ObjectStoreWireProtocol.ObjectStoreQueryStatus status in results)
				{
					Console.WriteLine("status = " + status.ToString());
				}
			}
		}


		public static void Delete(string pageKey)
		{
			using (var client = Client.Builder<Multimedia.PageDataKey, Multimedia.PageDataValue>(environment: EnvironmentEndpoint,
												  osNamespace: NamespaceName,
												  osTable: TableName,
												  timeout: new TimeSpan(0, 0, 0, 500),
												  maxRetries: 1).Create())
			{
				var key = new Multimedia.PageDataKey
				{
					PageKey = pageKey
				};

				var task = client.Delete<Multimedia.PageDataKey, Multimedia.PageDataValue>(key).WithDebugInfoEnabled().SendAsyncWithDebugFullResponse();
				try
				{
					task.Wait();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Delete() - Exception: {0}, {1}", ex.Message, ex.StackTrace);
				}

				var result = task.Result;

				Console.WriteLine("result = " + result.ToString());

				var status = task.Status;

				Console.WriteLine("status = " + status.ToString());                
			}
		}
	}
}
