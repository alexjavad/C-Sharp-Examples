# The Armstrong Freight Tracking System
The code below is entirely my own personal work written by myself, Alex Javad.
Some files contain too many lines of code for a full screenshot. 
For readability, I’ve uploaded most of these files to GitHub. 
Feel free to examine my work here: https://github.com/alexjavad/C-Sharp-Examples
I’ll make direct reference to some of these files in this document.


The following User Interface/Feature was built for Armstrong Transport Group, Inc. – a logistics company based in Concord, North Carolina. This is an integration with the Google Maps API, on the front-end, as well as the MacroPoint API on the backend. The MacroPoint API basically uses a truck driver’s cell phone, and triangulates the driver’s location, so that the people in the company can keep track of shipment progress, to make sure freight is being delivered on time.




The following is the backend C# code that is sending the MacroPoint API the information it needs in order to track the truck driver’s cell phone. When you “initiate tracking” the API will regularly send updates back to a “callback API” providing current coordinates, city, and zipcode information of the driver. This is the information that we then hand to the Google Maps API, to plot points along the driver’s route. Note the map above shows the origin of the shipment, and the stops the driver must make on his/her route to the destination (A, B, C). You can also notice in the bottom right of the above photo, that we made the system show the driver’s current city and state, when we receive an update.



To view the entire code for the MacroPoint integration, please do so here: 
https://github.com/alexjavad/C-Sharp-Examples/blob/master/MacroPointService.cs 




Building the XML data to “Initiate Tracking” with the MacroPoint API on a Load via HTTP:


Actually issuing the XML payload to “Initiate Tracking” to the MacroPoint API:










































This is the “callback API” that is configured to receive location updates from MacroPoint, and store that as a “comment” on a Load:




You can see the entire LoadController (with various MacroPoint callback APIs) here:
https://github.com/alexjavad/C-Sharp-Examples/blob/master/LoadController.cs 







The following is a chart tracking the performance of the sales team by group, person, and manager.
The code below produces the user interface directly above this sentence. The pie charts seen above are Telerik/Kendo UI Controls. We used Vue.js to streamline JavaScript development, it is utilized in the code below. 


<style>
    .no-wrap { white-space: no-wrap; }

    .report-column{ 
        border-right: 1px solid #DDD; 
        min-height: 500px;
        background-color: transparent;
        transition: background-color 1s;
    }
    .report-column h4{
        font-weight: bold;
        border-bottom: 1px solid #CCC;
        color: #828282;
    }
    .report-column:hover{
        background-color: #F1F1F1;
    }

    .date-selector{
        border-bottom: 1px solid #CCC; 
    }
</style>
<script type="text/template" id="Tpl-Manager-Office-Revenue">

    <section>
        <div class="date-selector">
            <table align="center">
                <tr>
                    <td>
                        <button v-on:click="changeIndex('sub')" class="btn btn-xs previous-month" style="position: relative; top: -4px;"><span class="glyphicon glyphicon-chevron-left"></span></button>
                    </td>

                    <td class="currentMonth bold text-center" width="80%"><h5>{{months[selectIndex]}} {{selectYear}}</h5></td>

                    <td>
                        <button v-on:click="changeIndex('add')" class="btn btn-xs next-month" style="position: relative; top: -4px;"><span class="glyphicon glyphicon-chevron-right"></span></button>
                    </td>
                </tr>
            </table>
        </div>


        <div class="col-md-4 loading report-column">
            <!-- By Group -->
            <h4 id="groupLoadIcon" class="my-loading-icon">Revenue By Group</h4>

            <div class="text-muted text-center no-margin" v-if="RevenueByGroup.length == 0">No data available for this month</div>

             <div class='col-md-12' v-el:by-group-chart></div>
             <div class="col-md-12">
                 <table class="table">
                     <thead>
                         <tr>
                         <th>Group</th>
                         <th>Revenue</th>
                         <th>Profit</th>
                         </tr>
                     </thead>
                     <tr v-for="group in RevenueByGroup">
                         <td class="no-wrap">{{group.Name}}</td>
                         <td class="text-danger bold">{{group.Revenue | currency}}</td>
                         <td class="text-success bold">{{group.Profit | currency}}</td>
                     </tr>
                 </table>
            </div>
        </div>
        

        <div class="col-md-4 loading report-column">
            <!-- By Group -->
            <h4 class="my-loading-icon">Revenue By Agent</h4>

            <div class="text-muted text-center no-margin" v-if="RevenueByUser.length == 0">No data available for this month</div>

             <div class='col-md-12' v-el:by-user-chart></div>
             <div class="col-md-12">
                 <table class="table">
                     <thead>
                         <tr>
                         <th>Rep</th>
                         <th>Revenue</th>
                         <th>Profit</th>
                         </tr>
                     </thead>
                     <tr v-for="user in RevenueByUser">
                         <td class="no-wrap">{{user.Name}}</td>
                         <td class="text-danger bold">{{user.Revenue | currency}}</td>
                         <td class="text-success bold">{{user.Profit | currency}}</td>
                     </tr>
                 </table>
            </div>
        </div>

        <div class="col-md-4 loading report-column">
            <!-- By Group -->
            <h4>Revenue By Sales Manager</h4>

            <div class="text-muted text-center no-margin" v-if="RevenueBySalesManager.length == 0">No data available for this month</div>

             <div class='col-md-12' v-el:by-salesmanager-chart></div>
             <div class="col-md-12">
                 <table class="table">
                     <thead>
                         <tr>
                         <th>Rep</th>
                         <th>Revenue</th>
                         <th>Profit</th>
                         </tr>
                     </thead>
                     <tr v-for="user in RevenueBySalesManager">
                         <td class="no-wrap">{{user.Name}}</td>
                         <td class="text-danger bold">{{user.Revenue | currency}}</td>
                         <td class="text-success bold">{{user.Profit | currency}}</td>
                     </tr>
                 </table>
            </div>
        </div>
        
        <div class="clear"></div>


        <div>
            <!-- By Group -->
            <h4>Loads Covered</h4>

            <h4 class="text-muted text-center no-margin" v-if="LoadsCovered.length == 0">No data available for this month</h4>
           
             <div class='col-md-7 loading' v-el:loads-covered-chart></div>
             <div class="col-md-5">
                 <table class="table">
                     <tr v-for="user in LoadsCoveredByUser">
                         <td class="no-wrap">{{user.Name}}</td>
                         <td class="text-success bold">{{user.NumLoads | number}}</td>
                     </tr>
                 </table>
            </div>
        </div>
    </section>
</script>


<script type="text/javascript">

    ATG.Vue.Component.OfficeStats = Vue.extend({
        template: $("#Tpl-Manager-Office-Revenue").html(),

        data: function () {
            return {
                months: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                selectYear: new Date().getFullYear(),
                selectIndex: new Date().getMonth(),
                OfficeId: null
            }
        },

        ready: function () {
            var self = this;

            //fetch data for this month
            this.fetchData( this.selectIndex, this.selectYear);
        },

        watch: {
            'selectIndex': function (month, oldVal) {
                if (oldVal == 11 && month == 0)
                    this.$set("selectYear", ++this.selectYear)

                if (oldVal == 0 && month == 11)
                    this.$set("selectYear", --this.selectYear);

                //fetch data for new month
                this.fetchData(month, this.selectYear, this.OfficeId);
            }
        },

        methods: {

            fetchData: function (month, year, officeId) {                
                month++;
                var self = this;

                self.$set("OfficeId", officeId);

                $(self.$els.loadsCoveredChart)
                    .add(self.$els.byUserChart)
                    .add(self.$els.byGroupChart)
                    .add(self.$els.bySalesmanagerChart)
                    .each(function () {

                        if ($(this).data("kendoChart"))
                            $(this).empty().data("kendoChart").destroy();
                    });
                        
                


                //get loads covered by user
                var url = ATG.Utility.buildUrl("/Dashboard/GetLoadsCoveredByUser", { month: month, year: year, officeId: officeId });
                ATG.DashboardStore.get(url, '1h').done(function (data) {

                    var sortedData = self._sortData(data, "NumLoads");
                    self.$set("LoadsCoveredByUser", sortedData);

                    self.barGraph(sortedData, self.$els.loadsCoveredChart, "NumLoads");
                });


                //get revenue by user
                url = ATG.Utility.buildUrl("/Dashboard/GetRevenueByUser", { month: month, year: year, officeId: officeId })
                ATG.DashboardStore.get(url, '1h').done(function (data) {

                    var sortedData = self._sortData(data, "Profit");
                    self.$set("RevenueByUser", sortedData);

                    self.pieChart(sortedData, self.$els.byUserChart, "Profit");
                });

                url = ATG.Utility.buildUrl("/Dashboard/GetRevenueBySalesManager", { month: month, year: year, officeId: officeId })
                ATG.DashboardStore.get(url, '4h').done(function (data) {

                    var sortedData = self._sortData(data, "Profit");
                    self.$set("RevenueBySalesManager", sortedData);

                    self.pieChart(sortedData, self.$els.bySalesmanagerChart, "Profit");

                });

                //get revenue by group
                url = ATG.Utility.buildUrl("/Dashboard/GetRevenueByGroup", { month: month, year: year, officeId: officeId });
                ATG.DashboardStore.get(url, '4h').done(function (data) {

                    var sortedData = self._sortData(data, "Profit");
                    self.$set("RevenueByGroup", sortedData);

                    self.pieChart(sortedData, self.$els.byGroupChart, "Profit");

                });


            },

            changeIndex: function (dir) {
                (dir == "add")
                        ? this.$set("selectIndex", ++this.selectIndex % 12)
                        : this.$set("selectIndex", (this.selectIndex == 0) ? 11 : --this.selectIndex);
            },


            pieChart: function (dataArray, chartId, field) {
                var data = [];
                var sumTotal = _.sum(_.pluck(dataArray, field));
                var self = this;

                
                _.each(dataArray, function (item) {
                    var val = ((item[field] / sumTotal) * 100).toFixed(1);
                    data.push({ category: item.Name, value: val });
                });

                $(chartId).kendoChart({
                    seriesColors: ["#00b300", "#00cc99", "#60b644", "#656338", "#00ccff", "#0f497b", "#080b1c",
                                    "#ff0044", "#8e00ff", "#7a8b8b"],
                    legend: {
                        position: "left"
                    },
                    seriesDefaults: {
                        labels: {
                            visible: true,
                            format: "{0}%"
                        }
                    },
                    series: [{
                        type: "pie",
                        data: data
                    }],
                    tooltip: {
                        visible: true,
                        format: "N0",
                        template: "#= category # - #= kendo.format('{0:P}', percentage)#"
                    }
                });
            },

            barGraph: function (dataArray, chartId, field) {
                var data = [];
                var sumTotal = _.sum(_.pluck(dataArray, field));
                var self = this;

                _.each(dataArray, function (item) {
                    data.push({ category: item.Name, value: item[field] });
                });

                $(chartId).kendoChart({
                    seriesColors: ["#00b300", "#00cc99", "#60b644", "#656338", "#00ccff", "#0f497b", "#080b1c",
                                    "#ff0044", "#8e00ff", "#7a8b8b"],
                    seriesDefaults: {
                        labels: {
                            visible: true,
                            format: "{0}"
                        }
                    },
                    series: [{
                        type: "bar",
                        data: data
                    }],
                    categoryAxis:{
                        categories: _.pluck(dataArray,"Name")
                    }
                });
            },


            _sortData: function (dataArray, field) {
                var self = this;

                var sortedData = _.sortBy(dataArray, function (item) {
                    return item[field];
                });

                return sortedData.reverse();
            }
        },
    });
</script>




RESTful Web Services for Armstrong Dashboard:















… and Associated Business Later/Data Access Layer:















… and the Associated Business Logic/Data Access Layer:




To view this entire file please visit:
https://github.com/alexjavad/C-Sharp-Examples/blob/master/DashboardBusiness.cs 








Dashboard related SQL stored Procedures:







Please let me know if you have any questions regarding the above.

