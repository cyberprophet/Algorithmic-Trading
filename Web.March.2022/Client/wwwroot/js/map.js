function initMap(location)
{
    map = new google.maps.Map(document.getElementById("map"), {
        center: {
            lat: latitude,
            lng: longitude
        },
        zoom: 15,
        styles: [
            {
                elementType: "geometry",
                stylers: [
                    {
                        color: "#242f3e"
                    }
                ]
            },
            {
                elementType: "labels.text.stroke",
                stylers: [
                    {
                        color: "#242f3e"
                    }
                ]
            },
            {
                elementType: "labels.text.fill",
                stylers: [
                    {
                        color: "#746855"
                    }
                ]
            },
            {
                featureType: "administrative.locality",
                elementType: "labels.text.fill",
                stylers: [
                    {
                        color: "#d59563"
                    }
                ]
            },
            {
                featureType: "poi",
                elementType: "labels.text.fill",
                stylers: [
                    {
                        color: "#d59563"
                    }
                ]
            },
            {
                featureType: "poi.park",
                elementType: "geometry",
                stylers: [
                    {
                        color: "#263c3f"
                    }
                ]
            },
            {
                featureType: "poi.park",
                elementType: "labels.text.fill",
                stylers: [
                    {
                        color: "#6b9a76"
                    }
                ]
            },
            {
                featureType: "road",
                elementType: "geometry",
                stylers: [
                    {
                        color: "#38414e"
                    }
                ]
            },
            {
                featureType: "road",
                elementType: "geometry.stroke",
                stylers: [
                    {
                        color: "#212a37"
                    }
                ]
            },
            {
                featureType: "road",
                elementType: "labels.text.fill",
                stylers: [
                    {
                        color: "#9ca5b3"
                    }
                ]
            },
            {
                featureType: "road.highway",
                elementType: "geometry",
                stylers: [
                    {
                        color: "#746855"
                    }
                ]
            },
            {
                featureType: "road.highway",
                elementType: "geometry.stroke",
                stylers: [
                    {
                        color: "#1f2835"
                    }
                ]
            },
            {
                featureType: "road.highway",
                elementType: "labels.text.fill",
                stylers: [
                    {
                        color: "#f3d19c"
                    }
                ]
            },
            {
                featureType: "transit",
                elementType: "geometry",
                stylers: [
                    {
                        color: "#2f3948"
                    }
                ]
            },
            {
                featureType: "transit.station",
                elementType: "labels.text.fill",
                stylers: [
                    {
                        color: "#d59563"
                    }
                ]
            },
            {
                featureType: "water",
                elementType: "geometry",
                stylers: [
                    {
                        color: "#17263c"
                    }
                ]
            },
            {
                featureType: "water",
                elementType: "labels.text.fill",
                stylers: [
                    {
                        color: "#515c6d"
                    }
                ]
            },
            {
                featureType: "water",
                elementType: "labels.text.stroke",
                stylers: [
                    {
                        color: "#17263c"
                    }
                ]
            }
        ]
    })
    if (location instanceof (Array))
    {
        location.forEach((item, index, array) =>
        {
            marker = new google.maps.Marker({
                icon:
                {
                    url: item.png,
                    scaledSize: new google.maps.Size(37.5, 37.5),
                    labelOrigin: new google.maps.Point((item.name.length - 1) / 2 * -14.5, 25)
                },
                optimized: true,
                position: item.position,
                animation: google.maps.Animation.DROP,
                title: item.code
            })
            marker.addListener('click', (e) =>
            {
                let m = markers.find(o => o.position === e.latLng)

                if (m instanceof (google.maps.Marker))
                {
                    let animation = m.getAnimation()

                    if (animation !== null && animation !== undefined)
                    {
                        m.setAnimation(null)
                    }
                    else
                    {
                        m.setAnimation(google.maps.Animation.BOUNCE)
                    }
                    recall.invokeMethodAsync('StateHasChanged', m.title)
                }
            })
            marker.addListener('mouseout', (e) =>
            {
                let m = markers.find(o => o.position === e.latLng)

                if (m instanceof (google.maps.Marker) && google.maps.Animation.BOUNCE === m.getAnimation())
                {
                    console.log(e.latLng)
                }
            })
            marker.addListener('mouseover', (e) =>
            {
                let m = markers.find(o => o.position === e.latLng)

                if (m instanceof (google.maps.Marker))
                {
                    let w = windows.find(o => o.code === m.title).window

                    if (w instanceof (google.maps.InfoWindow) && google.maps.Animation.BOUNCE !== m.getAnimation())
                    {
                        showContentWindows(m, w)
                    }
                    if (m.getLabel() === null || m.getLabel() === undefined)
                    {
                        m.setLabel({
                            text: item.name,
                            color: item.classification ? '#cc7000' : '#e67200',
                            fontWeight: '900',
                            fontSize: '15px',
                            fontFamily: 'Consolas'
                        })
                    }
                }
            })
            windows.push(new Info(item.code, new google.maps.InfoWindow({
                disableAutoPan: true,
                content: makeContentWindows(item.contents)
            })))
            markers.push(marker)
        })
    }
    if (markers instanceof (Array) && markers.length > 0)
    {
        new markerClusterer.MarkerClusterer({
            clusterOptions:
            {
                averageCenter: false,
                zoomOnClick: true
            },
            markers,
            map
        })
    }
    map.addListener('zoom_changed', () => recall.invokeMethodAsync('StateHasChanged', 'zoom_changed'))
    map.addListener('idle', () => recall.invokeMethodAsync('StateHasChanged', 'idle'))
}
function setContentWindows(isShown, arg)
{
    let w = windows.find(o => o.code === arg.code).window

    if (w instanceof (google.maps.InfoWindow))
    {
        w.setContent(makeContentWindows(arg))

        if (isShown)
        {
            let m = markers.find(o => o.title === arg.code)

            if (m instanceof (google.maps.Marker))
            {
                showContentWindows(m, w)
            }
        }
    }
}
function showContentWindows(marker, window)
{
    new Promise(() =>
    {
        window.open({
            anchor: marker,
            shouldFocus: false,
            map
        })
        setTimeout(() =>
        {
            let div = document.getElementById('reaction-contents-' + marker.title)

            if (cssRule === false)
            {
                let sheets = document.styleSheets

                for (var i = 0; i < sheets.length; i++)
                {
                    if (sheets[i] instanceof (CSSStyleSheet))
                    {
                        for (var j = 0; j < sheets[i].cssRules.length; j++)
                        {
                            if (sheets[i].cssRules[j] instanceof (CSSRule) && sheets[i].cssRules[j].selectorText === '.gm-style .gm-style-iw-t::after')
                            {
                                cssRule = sheets[i].deleteRule(j) === undefined

                                break;
                            }
                        }
                    }
                    if (cssRule)
                    {
                        break;
                    }
                }
            }
            if (div instanceof (HTMLDivElement))
            {
                let parent = div.parentElement

                if (parent instanceof (HTMLDivElement))
                {
                    let dialog = parent.parentElement
                    parent.style.overflow = 'unset'

                    if (dialog instanceof (HTMLDivElement))
                    {
                        dialog.childNodes.forEach((item, index, array) =>
                        {
                            if (item instanceof (HTMLButtonElement))
                            {
                                item.style.display = 'none'
                            }
                        })
                        dialog.style.overflow = 'unset'
                        dialog.style.backgroundColor = 'transparent'
                        dialog.style.boxShadow = ''
                        dialog.style.borderRadius = ''
                    }
                }
            }
        }, 4)
        setTimeout(() => window.close(), 2.35 * 1024)
    })
        .catch(error => console.log(error))
}
function makeContentWindows(arg)
{
    let container = document.createElement('div')

    if (container instanceof (HTMLDivElement))
    {
        container.setAttribute('class', 'reaction-contents')
        container.setAttribute('id', 'reaction-contents-' + arg.code)
        container.style.setProperty('--set-after-content', '"' + arg.after + '"')
        container.style.setProperty('--set-before-content', '"' + arg.before + '"')
        container.innerHTML = arg.html

        if (arg.sign === '1' || arg.sign === '2')
        {
            container.style.color = '#f00'
            container.style.textShadow = '.75px .75px #800000'
        }
        else if (arg.sign === '4' || arg.sign === '5')
        {
            container.style.color = '#00bfff'
            container.style.textShadow = '.75px .75px #000080'
        }
    }
    return container
}
function setIcon(code, url)
{
    if (markers instanceof (Array) && markers.length > 0)
    {
        let marker = markers.find(o => o.title === code)

        if (marker instanceof (google.maps.Marker))
        {
            let icon = marker.getIcon()

            if (url !== icon.url)
            {
                marker.setIcon({
                    url: url,
                    scaledSize: icon.scaledSize,
                    labelOrigin: icon.labelOrigin
                })
            }
        }
    }
}
function panTo(code, latitude, longitude)
{
    let m = markers.find(o => o.title === code)

    if (m instanceof (google.maps.Marker))
    {
        let animation = m.getAnimation()

        if (animation !== null && animation !== undefined)
        {
            m.setAnimation(null)
        }
        else
        {
            m.setAnimation(google.maps.Animation.BOUNCE)
        }
        recall.invokeMethodAsync('StateHasChanged', code)
    }
    map.panTo(new google.maps.LatLng(latitude, longitude))
}
function geolocation(reference)
{
    if (navigator.geolocation)
    {
        navigator.geolocation.getCurrentPosition((position) =>
        {
            var pc = position.coords
            longitude = pc.longitude
            latitude = pc.latitude
        },
            (error) => console.log(error.message))
    }
    else
    {
        longitude = 127.013685
        latitude = 37.294779
    }
    markers = []
    windows = []
    recall = reference
    cssRule = false
}
class Info
{
    constructor (code, window)
    {
        this.code = code
        this.window = window
    }
}
let map, longitude, latitude, markers, recall, windows, cssRule