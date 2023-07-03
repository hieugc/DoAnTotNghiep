function GetDataModel() {
    $.get(
        window.location.origin + "/Admin/GetSquare",
        function (data) {
            console.log(data);
            $(".num_squared").html(data.squared);
            $(".num_error").html(data.meanError);
            ModelChart(data.dataTest, data.dataPredict);
        }
    );
}
function ModelChart(dataTest, dataPredict) {
    for (e in dataTest) {
        dataTest[e] = parseInt(dataTest[e] / 1000);
    }
    for (e in dataPredict) {
        dataPredict[e] = parseInt(dataPredict[e] / 1000);
    }
    let cardColor, headingColor, axisColor, shadeColor, borderColor;
    cardColor = config.colors.white;
    headingColor = config.colors.headingColor;
    axisColor = config.colors.axisColor;
    borderColor = config.colors.borderColor;
    const totalRevenueChartEl = document.querySelector('#modelChart'),
        totalRevenueChartOptions = {
            chart: {
                height: 600,
                type: 'line',
                toolbar: {
                    show: false
                },
                dropShadow: {
                    enabled: true,
                    top: 10,
                    left: 5,
                    blur: 3,
                    color: config.colors.dark,
                    opacity: 0.15
                },
                sparkline: {
                    enabled: true
                }
            },
            grid: {
                show: false,
                padding: {
                    right: 8
                }
            },
            colors: [config.colors.primary, config.colors.warning],
            dataLabels: {
                enabled: false
            },
            stroke: {
                curve: 'smooth',
                width: 3
            },
            series: [
                {
                    name: 'Giá truyền vào',
                    data: dataTest
                },
                {
                    name: 'Giá dự đoán',
                    data: dataPredict
                }
            ],
            xaxis: {
                labels: {
                    style: {
                        fontSize: '13px',
                        colors: axisColor
                    }
                },
                axisTicks: {
                    show: false
                },
                axisBorder: {
                    show: false
                }
            },
            yaxis: {
                labels: {
                    style: {
                        fontSize: '13px',
                        colors: axisColor
                    }
                }
            },
            legend: {
                show: true,
                horizontalAlign: 'left',
                position: 'top',
                markers: {
                    height: 8,
                    width: 8,
                    radius: 12,
                    offsetX: -3
                },
                labels: {
                    colors: axisColor
                },
                itemMargin: {
                    horizontal: 10
                }
            },
            grid: {
                borderColor: borderColor,
                strokeDashArray: 4
            }
        };
    if (typeof totalRevenueChartEl !== undefined && totalRevenueChartEl !== null) {
        const totalRevenueChart = new ApexCharts(totalRevenueChartEl, totalRevenueChartOptions);
        totalRevenueChart.render();
    }
}