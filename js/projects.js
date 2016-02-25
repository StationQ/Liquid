function projects(projects)
{
	this.projects = projects;
	this.filteredProjects = [];
	this.count = 0;

	this.filterProjects = function(query, commitFilter){

	    if (query == "" && commitFilter == -1)
	    {
	    	this.filteredProjects = this.projects;
	    	return;
	    }

	    this.filteredProjects = [];

	    for(var p in this.projects) {

	    	var proj = this.projects[p].project;
	    	// determine if query matches, as empty string, repo name or contributor name
	      	if(query == "" || proj.Name.toLowerCase().indexOf(query) >= 0 || proj.Contributor.toLowerCase().indexOf(query) >=0) {
		      	// if no time filter, just add
		      	if (commitFilter == -1)
		      	{
		        	this.filteredProjects.push(this.projects[p]);
		        }
		        else
		        {
		        	var commit = moment(proj.CommitLast);
		        	var filterDate = moment().subtract(commitFilter,'days')
		        	if (commit >= filterDate)
		        	{
		        		this.filteredProjects.push(this.projects[p]);
		        	}
		        }
      		}
    	}
	};

	this.getPage = function(page, countPerPage) {

		if (page == null || countPerPage == null)
		{
			return this.filteredProjects;
		}

		var start = (page-1) * countPerPage;
		var end = start + countPerPage;

		if (start == 0 && end >= this.filteredProjects.length)
		{
			return this.filteredProjects;
		}
		else 
		{
			return this.filteredProjects.slice(start,end);
		}
	};
}