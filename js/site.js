require(["jquery"], function ($) {
    $(document).ready(function () {
    	var projectsTopUrl = "https://dnfrepos.blob.core.windows.net/output/projects_top.json";
    	var projectsUrl = "https://dnfrepos.blob.core.windows.net/output/projects.json"; 

    	$.getJSON(projectsTopUrl, function(data)
    	{
			require(["moment","ko"], function (moment,ko) {

				var timeFilters = [{"name": "Unfiltered", "days":-1}, {"name": "Last 24 hours", "days": 1},{"name" :"Last 7 days", "days" : 8} , {"name": "Last month", "days": 31}];
				var minProjects = 30;
				var config = true;
				var p = new projects(convertProjects(data.Projects));

				ko.observable.fn.toggle = function () {
				    var obs = this;
				    return function () {
				        obs(!obs())
				    };
				};

	    		var viewModel = {
				    summary: data.Summary,
				    projects : ko.observableArray([]),
				    query: ko.observable('').extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } }),
					pages: ko.observableArray([]),
					selectedPage: ko.observable(1),
					timeFilters: timeFilters,
					selectedTimeFilter: ko.observable(),
					selectedTimeFilterInDays: -1,
					countFilter : ko.observable(true),
					countFilters : ko.observableArray([]),
					selectedCountFilter: ko.observable(minProjects),
					filterByTime: function(value)
					{
				    	//console.log('filterByTime');
						if (config) 
							{
								//console.log("filter no-op");
								return;
							}
					    var index = indexOf(viewModel.timeFilters,value.name);
					    viewModel.selectedTimeFilterInDays = viewModel.timeFilters[index].days;
				    	filterProjects();
					},
					filter: function(value) {
						//console.log("filter");
						if (config) 
							{
								//console.log("search no-op");
								return;
							}
				    	filterProjects();
					},
					gotoRepo: function() {
				    	window.location.href=this.project.Url;
					},
					gotoPage: function(value) {
						viewModel.selectedPage(value.number);
						pageProjects();
					},
					nextPage: function(value)
					{
						var currentPage = viewModel.selectedPage();
						currentPage = currentPage + value;
						if (currentPage < 1 || currentPage > viewModel.pages().length) return;
						viewModel.selectedPage(currentPage);
						pageProjects();
					}
				};

				ko.bindingHandlers.datetime = {
				    update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
				    	var value = valueAccessor();
				    	var valueUnwrapped = ko.unwrap(value);
				    	element.innerHTML = moment(valueUnwrapped).format('MMM. DD, YYYY')
				    }
				};

				viewModel.query.subscribe(viewModel.filter);
				viewModel.selectedCountFilter.subscribe(viewModel.filter);				
				viewModel.selectedTimeFilter.subscribe(viewModel.filterByTime);
				//console.log("apply bindings");
				ko.applyBindings(viewModel);
				config = false;
				configureCountFilter(data.Projects.length);
				filterProjects();				

				
				$.getJSON(projectsUrl, function(data)
    			{
    				configureCountFilter(data.Projects.length);
    				p = new projects(convertProjects(data.Projects));
    				filterProjects();
    			});


    			function filterProjects()
    			{   
    				var query = viewModel.query().toLowerCase();
    				var commitQuery = viewModel.selectedTimeFilterInDays;
    				p.filterProjects(query, commitQuery);
				    viewModel.projects(p.getPage(1,viewModel.selectedCountFilter()));
    				configurePaging(p.filteredProjects.length);		
    				//console.log("projects length: " + viewModel.projects().length);		    
    			}

    			function pageProjects()
    			{
				    viewModel.projects(p.getPage(viewModel.selectedPage(),viewModel.selectedCountFilter()));
    				configurePaging(p.filteredProjects.length);	
    				//console.log("projects length: " + viewModel.projects().length);			    
    			}

				function project(project)
				{
					this.project = project;
					this.summaryVisible = ko.observable(true);
				}

				function page(number, selected)
				{
					this.number = number;
					this.selected = selected;
				}

				function convertProjects(projects)
				{
					var wrappedProjects = [];
					for (var p in projects)
					{
						wrappedProjects.push(new project(projects[p]));
					}
					return wrappedProjects;
				}

				function indexOf(a, v) {
				    for (var i in a)
				    {
				        if (a[i].name == v)
				        {
				            return i;
				        }
				    }
				    return -1;
				}

				function configureCountFilter(count)
				{
					if (count <minProjects)
					{
						//console.log("< min projects");
						viewModel.countFilter(false);
					}
					else if (count > 400)
					{					
						viewModel.countFilters([minProjects, 90, 300, count]);
						viewModel.countFilter(true);
					}					
					else if (count > 120)
					{					
						viewModel.countFilters([minProjects, 90, count]);
						viewModel.countFilter(true);
					}
					else 
					{
						viewModel.countFilters([minProjects, count]);
						viewModel.countFilter(true);
					}
				}

				function configurePaging(count)
				{
					var pageCount;
					var pages = [];
					var selectedCount = viewModel.selectedCountFilter();
					var selectedPage = viewModel.selectedPage();
					pageCount = count / selectedCount;

					pageCount = Math.ceil(pageCount);

					if (pageCount <2)
					{
						viewModel.countFilter(false);
						return;
					}

					pageCount++;
					for (i = 1; i < pageCount; i++)
					{
						var selected = false;
						if (i == selectedPage)
						{
							selected = true;
						}
						pages.push(new page(i, selected));
					}
					viewModel.pages(pages);
					viewModel.countFilter(true);
				}
			});
    	});
    });
});


