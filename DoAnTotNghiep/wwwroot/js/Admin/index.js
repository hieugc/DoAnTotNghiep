function getUser(option) {
    $.get(
        window.location.origin + "/Admin/User?option=" + encodeURIComponent(option),
        function (data) {
            $(".num_user").html(data.increase);
            $(".cent_user").html(`<i class="bx bx-up-arrow-alt"></i>${data.pourCent} người`);
        }
    );
}
function getHouse(option) {
    $.get(
        window.location.origin + "/Admin/House?option=" + encodeURIComponent(option),
        function (data) {
            $(".num_house").html(data);
        }
    );
}
function getFeedBack(option) {
    $.get(
        window.location.origin + "/Admin/FeedBack?option=" + encodeURIComponent(option),
        function (data) {
            $(".num_feedback").html(data);
        }
    );
}
function getRequest(option) {
    $.get(
        window.location.origin + "/Admin/Request?option=" + encodeURIComponent(option),
        function (data) {
            $(".num_request").html(data.total);
            $(".num_request_accept").html(data.accept);
            $(".num_request_reject").html(data.reject);
        }
    );
}
function getReport(option) {
    $.get(
        window.location.origin + "/Admin/Report?option=" + encodeURIComponent(option),
        function (data) {
            $(".num_report").html(data.increase);
            $(".cent_report").html(`<i class="bx bx-up-arrow-alt"></i>${data.pourCent} bài viết`);
        }
    );
}

function Income(months, data) {
    let cardColor, headingColor, axisColor, shadeColor, borderColor;
    cardColor = config.colors.white;
    headingColor = config.colors.headingColor;
    axisColor = config.colors.axisColor;
    borderColor = config.colors.borderColor;

    const incomeChartEl = document.querySelector('#incomeChart'),
        incomeChartConfig = {
            series: [
                {
                    name: 'Tổng giá trị',
                    data: data
                }
            ],
            chart: {
                height: 350,
                parentHeightOffset: 0,
                parentWidthOffset: 0,
                toolbar: {
                    show: false
                },
                type: 'area'
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                width: 2,
                curve: 'smooth'
            },
            legend: {
                show: false
            },
            markers: {
                size: 6,
                colors: 'transparent',
                strokeColors: 'transparent',
                strokeWidth: 4,
                discrete: [
                    {
                        fillColor: config.colors.white,
                        seriesIndex: 0,
                        dataPointIndex: data.length - 1,
                        strokeColor: config.colors.primary,
                        strokeWidth: 2,
                        size: 6,
                        radius: 8
                    }
                ],
                hover: {
                    size: data.length - 1
                }
            },
            colors: [config.colors.primary],
            fill: {
                type: 'gradient',
                gradient: {
                    shade: shadeColor,
                    shadeIntensity: 0.6,
                    opacityFrom: 0.5,
                    opacityTo: 0.25,
                    stops: [0, 95, 100000]
                }
            },
            grid: {
                borderColor: borderColor,
                strokeDashArray: 3,
                padding: {
                    top: -20,
                    bottom: -8,
                    left: -10,
                    right: 8
                }
            },
            xaxis: {
                categories: months,
                axisBorder: {
                    show: false
                },
                axisTicks: {
                    show: false
                },
                labels: {
                    show: true,
                    style: {
                        fontSize: '13px',
                        colors: axisColor
                    }
                }
            },
            yaxis: {
                labels: {
                    show: false
                },
                min: 10,
                tickAmount: 4
            }
    };
    if (typeof incomeChartEl !== undefined && incomeChartEl !== null) {
        const incomeChart = new ApexCharts(incomeChartEl, incomeChartConfig);
        incomeChart.render();
    }
}

function getTransaction(option) {
    $.get(
        window.location.origin + "/Admin/Transaction?option=" + encodeURIComponent(option),
        function (data) {
            console.log(data);
            let label = [];
            let trandata = [];
            let month_temp = 0;
            let total = 0;
            for (e in data) {
                if (month_temp != data[e].month) {
                    label[label.length] = "Tháng " + data[e].month;
                    month_temp = data[e].month;
                    trandata[trandata.length] = total;
                    total = 0;
                }
                else {
                    total += data[e].value;
                }
            }
            console.log(label);
            console.log(trandata);
            Income(label, trandata);
        }
    );
}
getTransaction(0);