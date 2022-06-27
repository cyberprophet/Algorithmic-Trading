function drawCandlestickChart(args, name)
{
    const period = 'candlestick-period', chart = 'candlestick-chart'
    let chartElement = document.getElementById(chart)

    if (chartElement instanceof (HTMLDivElement) && document.getElementById(period) instanceof (HTMLDivElement))
    {
        let dashboard = new google.visualization.Dashboard()
        let slider = new google.visualization.ControlWrapper({
            controlType: 'ChartRangeFilter',
            options:
            {
                ui:
                {
                    chartOptions:
                    {
                        chartArea:
                        {
                            width: '90%'
                        },
                        hAxis:
                        {
                            textStyle:
                            {
                                color: 'snow'
                            },
                            baselineColor: 'none',
                            format: 'yyyy-MM-dd'
                        },
                        backgroundColor:
                        {
                            fill: '#171b29',
                            fillOpacity: 0.9
                        },
                        curveType: 'function',
                        fontSize: 7
                    },
                    chartType: 'LineChart'
                },
                filterColumnIndex: 0
            },
            containerId: period
        })
        let wrapper = new google.visualization.ChartWrapper({
            chartType: 'CandlestickChart',
            containerId: chart,
            options:
            {
                aggregationTarget: 'auto',
                candlestick:
                {
                    fallingColor:
                    {
                        strokeWidth: 3,
                        stroke: '#00bfff',
                        fill: '#00bfff'
                    },
                    risingColor:
                    {
                        strokeWidth: 3,
                        stroke: '#f00',
                        fill: '#f00'
                    }
                },
                tooltip:
                {
                    textStyle:
                    {
                        fontName: 'Consolas',
                        fontSize: 10
                    },
                    showColorCode: true
                },
                hAxis:
                {
                    textStyle:
                    {
                        color: 'snow'
                    },
                    gridlines:
                    {
                        color: '#808080',
                        minSpacing: 50
                    },
                    minorGridlines:
                    {
                        color: '#696969',
                        minSpacing: 50
                    },
                    slantedText: false,
                    format: 'yyyy-MM-dd',
                    title: ''
                },
                vAxis:
                {
                    textStyle:
                    {
                        color: 'snow'
                    },
                    gridlines:
                    {
                        color: '#808080',
                        minSpacing: 50
                    },
                    minorGridlines:
                    {
                        color: '#696969',
                        minSpacing: 50
                    },
                    title: ''
                },
                chartArea:
                {
                    width: '90%',
                    height: '82.5%'
                },
                titleTextStyle:
                {
                    fontSize: 12,
                    color: '#CCC'
                },
                backgroundColor:
                {
                    fill: '#171b29',
                    fillOpacity: 0.9
                },
                legend: 'none',
                series:
                {
                    0:
                    {
                        targetAxisIndex: 1
                    }
                },
                colors: ['snow'],
                title: name,
                fontSize: 9
            }
        })
        args.rows.map(array => array.c[0].v = new Date(array.c[0].v))
        let data = new google.visualization.DataTable(args)
        formatter = new google.visualization.DateFormat({
            pattern: 'yyyy-MM-dd'
        });
        formatter.format(data, 0);
        dashboard.bind(slider, wrapper)
        let element = document.getElementById('grid-container')
        let timer = null

        if (element instanceof (HTMLDivElement))
        {
            let accContainer = element.getElementsByClassName('grid-chart-scroller')

            if (accContainer.length === 1)
                new ResizeObserver(resize).observe(accContainer[0])
        }
        function resize()
        {
            clearTimeout(timer)
            timer = setTimeout(() => dashboard.draw(data), 300)
        }
        google.visualization.events.addListener(dashboard, 'ready', () =>
        {
            chartElement.querySelectorAll('rect[fill="#ff0000"]').forEach((item, index, arr) =>
            {
                item.previousSibling.style.fill = '#f00'
            })
            chartElement.querySelectorAll('rect[fill="#00bfff"]').forEach((item, index, arr) =>
            {
                item.previousSibling.style.fill = '#00bfff'
            })
        })
        candlestickBoard = dashboard
        candlestickData = data
    }
    console.log(args)
}
function draw(period, chart, args, name)
{
    if (document.getElementById(period) instanceof (HTMLDivElement) && document.getElementById(chart) instanceof (HTMLDivElement))
    {
        let dashboard = new google.visualization.Dashboard()
        let slider = new google.visualization.ControlWrapper({
            controlType: 'ChartRangeFilter',
            containerId: period,
            options:
            {
                ui:
                {
                    chartOptions:
                    {
                        chartArea:
                        {
                            width: '90%'
                        },
                        hAxis:
                        {
                            textStyle:
                            {
                                color: 'snow'
                            },
                            baselineColor: 'none',
                            format: 'yyyy-MM-dd'
                        },
                        backgroundColor:
                        {
                            fill: '#171b29',
                            fillOpacity: 0.9
                        },
                        fontSize: 7
                    },
                    chartType: 'AreaChart'
                },
                filterColumnIndex: 0
            }
        })
        let wrapper = new google.visualization.ChartWrapper({
            chartType: 'ColumnChart',
            containerId: chart,
            options:
            {
                chartArea:
                {
                    width: '90%',
                    height: '82.5%'
                },
                titleTextStyle:
                {
                    fontSize: 12,
                    color: '#CCC'
                },
                trendlines: {
                    0:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    },
                    1:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    },
                    2:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    },
                    3:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    },
                    4:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    }, 5:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    },
                    6:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    },
                    7:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    },
                    8:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    },
                    9:
                    {
                        type: 'polynomial',
                        degree: 3,
                        lineWidth: 2.5,
                        opacity: .7
                    }
                },
                legend:
                {
                    textStyle:
                    {
                        color: 'gold',
                        fontSize: 11
                    },
                    position: 'top'
                },
                tooltip:
                {
                    textStyle:
                    {
                        fontName: 'Consolas',
                        fontSize: 10
                    },
                    showColorCode: true
                },
                hAxis:
                {
                    textStyle:
                    {
                        color: 'snow'
                    },
                    gridlines:
                    {
                        color: '#808080',
                        minSpacing: 50
                    },
                    minorGridlines:
                    {
                        color: '#696969',
                        minSpacing: 50
                    },
                    slantedText: false,
                    format: 'yyyy-MM-dd',
                    title: ''
                },
                vAxis:
                {
                    textStyle:
                    {
                        color: 'snow'
                    },
                    gridlines:
                    {
                        color: '#808080',
                        minSpacing: 50
                    },
                    minorGridlines:
                    {
                        color: '#696969',
                        minSpacing: 50
                    },
                    title: ''
                },
                backgroundColor:
                {
                    fill: '#171b29',
                    fillOpacity: 0.9
                },
                fontSize: 9,
                isStacked: true,
                title: name
            }
        })
        args.rows.map(array => array.c[0].v = new Date(array.c[0].v))
        let data = new google.visualization.DataTable(args)
        formatter = new google.visualization.DateFormat({
            pattern: 'yyyy-MM-dd'
        });
        formatter.format(data, 0);
        dashboard.bind(slider, wrapper)
        let element = document.getElementById('grid-container')
        let timer = null

        if (element instanceof (HTMLDivElement))
        {
            let accContainer = element.getElementsByClassName('grid-account-scroller')

            if (accContainer.length === 1)
                new ResizeObserver(resize).observe(accContainer[0])
        }
        function resize()
        {
            clearTimeout(timer)
            timer = setTimeout(() => dashboard.draw(data), 300)
        }
        boards.push(new Account(name, dashboard, data))
    }
    console.log(args)
}
function setValues(name, row)
{
    if (boards instanceof (Array))
    {
        let val = 0
        let now = new Date(row.date)
        const total = boards.find(o => o.name === '')
        const acc = boards.find(o => o.name === name)

        if (acc instanceof (Account))
        {
            let length = acc.data.getNumberOfRows()
            let date = acc.data.getValue(length - 1, 0)
            let bf = acc.data.bf

            if (bf instanceof (Array))
            {
                const index = bf.findIndex(o => o.label === row.label)

                if (now.toDateString() === date.toDateString())
                {
                    val = acc.data.getValue(length - 1, index)
                    acc.data.setValue(length - 1, index, row.value)
                }
                else
                {
                    let arr = new Array(bf.length)
                    arr[0] = now
                    arr[index] = row.value
                    arr.map(value =>
                    {
                        if (value === null || value === undefined)
                        {
                            value = 0
                        }
                    })
                    acc.data.addRow(arr)
                    formatter.format(acc.data, 0)
                }
            }
            acc.dashboard.draw(acc.data)
        }
        if (total instanceof (Account))
        {
            let length = total.data.getNumberOfRows()
            let date = total.data.getValue(length - 1, 0)
            let bf = total.data.bf

            if (bf instanceof (Array))
            {
                const index = bf.findIndex(o => o.label === row.label)

                if (now.toDateString() === date.toDateString())
                {
                    total.data.setValue(length - 1, index, total.data.getValue(length - 1, index) - val + row.value)
                }
                else
                {
                    let arr = new Array(bf.length)
                    arr[0] = now
                    arr[index] = row.value
                    arr.map(value =>
                    {
                        if (value === null || value === undefined)
                        {
                            value = 0
                        }
                    })
                    total.data.addRow(arr)
                    formatter.format(total.data, 0)
                }
            }
            total.dashboard.draw(total.data)
        }
    }
}
function addRows()
{

}
function load(args)
{
    google.charts.load('current', args)
    console.log(args)
    boards = []
}
class Account
{
    constructor (name, dashboard, data)
    {
        this.name = name
        this.dashboard = dashboard
        this.data = data
    }
}
let boards, formatter, candlestickBoard, candlestickData