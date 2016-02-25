(function (ko, $, moment){
	
	var EventGroup = function (sourceObj) {
		//TODO add the checking logic here or something
		this.name = ko.observable(sourceObj.group_name);
		this.url = ko.observable(sourceObj.group_url);
	};
	
	var EventSource = function(sourceObj) {
		this.name = ko.observable();
		this.image = ko.observable();
		var cmLogo = "http://dotnetsocialweb.azurewebsites.net/assets/cm_logo.png";
	    var mLogo =  "http://dotnetsocialweb.azurewebsites.net/assets/meetup_logo.png";
	    var dLogo =  "http://dotnetsocialweb.azurewebsites.net/assets/net_logo.png";
		
		switch (sourceObj.source) {
			case "meetup.com":
				this.name("meetup");
				this.image(mLogo); 
				break;
			case "communitymegaphone.com":
				this.name("communitymegaphone");
				this.image(cmLogo);
				break;
			default: 
				this.name("none");
				this.image(dLogo);
				break;
		}
	};
	
	var Event = function(sourceObj) {
		this.name = ko.observable(sourceObj.name);
		this.url = ko.observable(sourceObj.event_url === "" ? "https://twitter.com/DotNet/dotnet-user-groups" : sourceObj.event_url);
		this.description = ko.observable(sourceObj.description);
		this.source = ko.observable(new EventSource(sourceObj));
		this.dateTime = ko.observable(moment(sourceObj.time).format('dddd, MMMM Do YYYY'));
		this.orgGroup = ko.observable(new EventGroup(sourceObj));
		
	};
	
	var ugAggregatorViewModel = {
	    // defaultUrl :"https://twitter.com/DotNet/dotnet-user-groups",
	    cmBaseUrl : "http://www.communitymegaphone.com",
	    meetupBaseUrl: "http://www.meetup.com/",
		loaded: ko.observable(false),
		events: ko.observableArray(),
		meetupCount: ko.observable(10),
		loadEvents: function () {
			 var self = this;
			 $.ajax({
				 url: "http://dotnetsocial.cloudapp.net/api/meetup",
				 method: "GET",
				 data: { count: self.meetupCount(), expiry: 60 }
			 })
			 .done(function (data) {
				 //console.log(data);
				 if (data.length > 0){
					 var tempResult = [];
					 $.each(data, function (key, value) {
						 //console.log(value);
						 tempResult.push(new Event(value));
					 });
					 self.events(tempResult);
					 //console.log(self.events());
					 self.loaded(true);
				 }
			 })
			 .fail(function (data) {
				console.log(data); 
			 });
			 
		 }
	};
	ugAggregatorViewModel.loadEvents();
	ko.applyBindings(ugAggregatorViewModel, document.getElementById("communityContainer"));
	
})(ko, $, moment);
