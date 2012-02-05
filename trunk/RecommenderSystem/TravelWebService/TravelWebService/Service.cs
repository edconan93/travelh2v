﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Data;
using System.Web.Script.Serialization;
using RecommenderSystem.Core.Helper;
using RecommenderSystem.Core.RS_Core;

namespace TravelWebService
{
    // Start the service and browse to http://<machine_name>:<port>/Service/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class Service
    {
        // TODO: Implement the collection resource that will contain the SampleItem instances
        // Get All Places
        [WebInvoke(UriTemplate = "GetAllPlaces", Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        [Description("list operation.")]
        public Stream  GetAllPlaces()
        {
            List<PlaceModel> places = PlaceModel.GetAllPlaces();
            var javaScriptSerializer = new JavaScriptSerializer();
            string jsonStringMultiple = "{\"responseData\":" + javaScriptSerializer.Serialize(places.Select(x => new { x.id, x.place_category_obj, x.name, x.imgurl, x.lat, x.lng, x.house_number, x.street, x.ward, x.district, x.city, x.province, x.country, x.phone_number, x.email, x.website, x.history, x.details, x.sources, x.general_rating, x.general_count_rating, x.general_sum_rating })) + "}";
            var json = Encoding.UTF8.GetBytes(jsonStringMultiple);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/javascript; charset=utf-8";                
            var memoryStream = new MemoryStream(json);
            return memoryStream;
        }
        
        // Rate a place
        [WebInvoke(UriTemplate = "Rate?username={username}&place={id_place}&weather={weather}&companion={companion}&budget={budget}&time={time}&rating={rating}", Method = "GET")]
        public bool Rate(string username, int id_place, int weather, int companion, int budget, string time, float rating)
        {
            // TODO: Add the new instance of SampleItem to the collection
            //throw new NotImplementedException();
            RatingModel r = new RatingModel();
            r.username = username;
            r.id_place = id_place;
            r.id_weather = weather;
            r.id_companion = companion;
            r.id_budget = budget;
            r.time = time.Replace("and"," ");
            r.rating = rating;
            return r.Rate();
        }

        // sugesstions
        [WebInvoke(UriTemplate = "Suggestions?username={username}&weather={weather}&companion={companion}&budget={budget}&time={time}", Method = "GET")]
        public Stream Suggestions(string username, int weather, int companion, int budget, string time)
        {
            // TODO: Add the new instance of SampleItem to the collection
            //throw new NotImplementedException();
            time = time.Replace("and", " ");
            List<Recommendation> recommendation = Recommendation.Recommend(username, weather, companion, budget, time);
            var javaScriptSerializer = new JavaScriptSerializer();
            string jsonStringMultiple = "{\"responseData\":" + javaScriptSerializer.Serialize(recommendation.Select(x => new { x.item, x.ratingEstimated})) + "}";
            var json = Encoding.UTF8.GetBytes(jsonStringMultiple);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/javascript; charset=utf-8";
            var memoryStream = new MemoryStream(json);
            return memoryStream;
        }
    }
}
