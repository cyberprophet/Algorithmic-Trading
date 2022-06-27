function setMarketTime(arg)
{
    let element = document.getElementById('hide-button-time')

    if (element instanceof (HTMLDivElement))
    {
        if (arg.change)
        {
            element.style.textShadow = getTextShadow(arg.sign)
            element.style.color = getColor(arg.sign)
        }
        else if (arg.sign !== '3' && element.style.color === '')
        {
            element.style.textShadow = getTextShadow(arg.sign)
            element.style.color = getColor(arg.sign)
        }
        element.innerText = arg.time
    }
}
function getColor(sign)
{
    switch (sign)
    {
        case '1':
        case '2':
            return 'red'

        case '4':
        case '5':
            return 'deepskyblue'

        default:
            return '#808080'
    }
}
function getTextShadow(sign)
{
    switch (sign)
    {
        case '1':
        case '2':
            return '.75px .75px #800000'

        case '4':
        case '5':
            return '.75px .75px #000080'

        default:
            return '.5px .5px #e0e0e0'
    }
}
function drawTheDailyChartBar(arg)
{
    let element = document.getElementById('grid-stock-item-' + arg.code)
    let res = true

    if (element instanceof (HTMLDivElement))
    {
        element.childNodes.forEach((item, index, array) =>
        {
            switch (index)
            {
                case 0:

                    if (item instanceof (HTMLDivElement) && arg.price !== null && arg.price !== undefined)
                    {
                        if (arg.change)
                        {
                            item.style.color = getColor(arg.sign)
                            item.style.textShadow = getTextShadow(arg.sign)
                        }
                        item.innerText = arg.price
                    }
                    break

                case 1:

                    if (item instanceof (HTMLDivElement) && arg.rate !== null && arg.rate !== undefined)
                    {
                        if (arg.change)
                        {
                            item.style.color = getColor(arg.sign)
                            item.style.textShadow = getTextShadow(arg.sign)
                        }
                        item.innerText = arg.rate
                    }
                    break

                case 2:

                    if (item instanceof (HTMLDivElement) && arg.volume !== null && arg.volume !== undefined)
                    {
                        item.innerText = arg.volume
                    }
                    break

                case 3:

                    if (item instanceof (HTMLDivElement) && arg.day !== null && arg.day !== undefined)
                    {
                        if (arg.change)
                        {
                            item.style.color = getColor(arg.sign)
                            item.style.textShadow = getTextShadow(arg.sign)
                        }
                        switch (arg.sign)
                        {
                            case '2':
                                item.innerHTML = '<span class="oi oi-caret-top oi-padding">' + '</span>' + arg.day
                                break;

                            case '5':
                                item.innerHTML = '<span class="oi oi-caret-bottom oi-padding">' + '</span>' + arg.day
                                break;

                            case '1':
                                item.innerHTML = '<span class="oi oi-arrow-thick-top oi-padding">' + '</span>' + arg.day
                                break;

                            case '4':
                                item.innerHTML = '<span class="oi oi-arrow-thick-bottom oi-padding">' + '</span>' + arg.day
                                break;

                            default:
                                item.innerText = arg.day
                                break;
                        }
                    }
                    break

                case 4:

                    if (item instanceof (HTMLDivElement) && arg.amount !== null && arg.amount !== undefined)
                    {
                        item.innerText = arg.amount
                    }
                    break

                case 6:

                    if (item.hasChildNodes())
                    {
                        item.childNodes.forEach((g, i, arr) =>
                        {
                            if (g instanceof (HTMLDivElement))
                            {
                                let color = arg.color > 0 ? '#f00' : '#00bfff'
                                let edge = arg.color > 0 ? '#800000' : '#00f'

                                switch (i)
                                {
                                    case 0:
                                        g.style.width = arg.bottom
                                        break

                                    case 2:
                                        g.style.background = 'linear-gradient(to right, ' + edge + ', ' + color + ')'
                                        g.style.width = arg.lowerTail
                                        break

                                    case 4:
                                        g.style.backgroundColor = color
                                        g.style.width = arg.body
                                        break

                                    case 6:
                                        g.style.background = 'linear-gradient(to right, ' + color + ', ' + edge + ')'
                                        g.style.width = arg.top
                                        break

                                    default:
                                        console.log(g)
                                        break;
                                }
                                res = false
                            }
                        })
                    }
                    break
            }
        })
    }
    return res
}
function setGridOpacity(value)
{
    let element = document.getElementById('grid-container'),
        grid = document.getElementById('float-end-container'),
        map = document.getElementById('map'),
        auth = document.getElementById('main-login-display'),
        media = window.matchMedia('screen and (max-width: 641px)')

    if (element instanceof (HTMLDivElement))
    {
        element.style.opacity = value
    }
    if (map instanceof (HTMLDivElement))
    {
        let gm_style_mtc = map.getElementsByClassName('gm-style-mtc'),
            gm_control_active = map.getElementsByClassName('gm-control-active gm-fullscreen-control'),
            gm_svpc = map.getElementsByClassName('gm-svpc'),
            gmnoprint = map.getElementsByClassName('gmnoprint')

        if (gmnoprint instanceof (HTMLCollection))
        {
            for (var i = 0; i < gmnoprint.length; i++)
            {
                if (gmnoprint[i].hasChildNodes)
                {
                    gmnoprint[i].childNodes.forEach((item, index, array) =>
                    {
                        if (item.hasChildNodes() && item instanceof (HTMLDivElement) && item.hasAttribute('class') === false)
                        {
                            item.childNodes.forEach((cItem, cIndex, arr) =>
                            {
                                if (cItem instanceof (HTMLButtonElement))
                                {
                                    cItem.style.backgroundColor = '#171b29'
                                    cItem.style.opacity = value
                                    cItem.style.borderRadius = cIndex === 0 ? '.5rem .5rem 0 0' : '0 0 .5rem .5rem'

                                    if (cIndex === 0)
                                    {
                                        item.style.backgroundColor = '#171b29'
                                        item.style.opacity = value
                                        item.style.borderRadius = '.5rem'
                                    }
                                }
                            })
                        }
                    })
                }
            }
        }
        if (gm_control_active instanceof (HTMLCollection) && gm_control_active.length === 1)
        {
            gm_control_active[0].style.backgroundColor = '#171b29'
            gm_control_active[0].style.opacity = value
            gm_control_active[0].style.borderRadius = '.5rem'

            if (media.matches)
            {
                gm_control_active[0].style.width = ''
                gm_control_active[0].style.height = ''
            }
        }
        if (gm_svpc instanceof (HTMLCollection) && gm_svpc.length === 1)
        {
            gm_svpc[0].style.backgroundColor = '#171b29'
            gm_svpc[0].style.opacity = value
            gm_svpc[0].style.borderRadius = '.5rem'
        }
        if (gm_style_mtc instanceof (HTMLCollection))
        {
            for (var i = 0; i < gm_style_mtc.length; i++)
            {
                if (gm_style_mtc[i].hasChildNodes())
                {
                    gm_style_mtc[i].childNodes.forEach((item, index, arr) =>
                    {
                        if (item instanceof (HTMLButtonElement))
                        {
                            item.style.color = 'darkgray'
                            item.style.backgroundColor = '#171b29'
                            item.style.opacity = value
                            item.style.borderRadius = i === 0 ? '.5rem 0 0 .5rem' : '0 .5rem .5rem 0'

                            if (media.matches)
                            {
                                item.style.padding = '3px'
                                item.style.height = ''
                                item.style.fontSize = ''
                            }
                        }
                    })
                }
            }
        }
    }
    if (auth instanceof (HTMLDivElement))
    {
        auth.style.background = 'linear-gradient(#171b29, #242f3e)'
        auth.style.color = 'snow'
    }
    if (grid instanceof (HTMLDivElement))
    {
        grid.style.opacity = value
    }
}
function disable(id)
{
    let element = document.getElementById(id)

    if (element instanceof (HTMLButtonElement))
    {
        element.disabled = element.disabled === false
    }
}
function setMainDisplayMessage(message)
{
    let element = document.getElementById('main-login-display-message')

    if (element instanceof (HTMLSpanElement))
    {
        element.style.fontWeight = 'lighter'
        element.innerText = message
    }
}
function setResizeTimer(delay)
{
    let element = document.getElementById('grid-container')

    if (element instanceof (HTMLDivElement))
    {
        let collection = element.getElementsByClassName('grid-scroller')
        let timer

        if (collection instanceof (HTMLCollection) && collection.length === 1)
        {
            new ResizeObserver(() =>
            {
                clearTimeout(timer)
                timer = setTimeout(() => console.log(element.children.length), delay)
            })
                .observe(collection[0])
        }
    }
}
function setQuotesTime(id, arg)
{
    let element = document.getElementById(id)

    if (element instanceof (HTMLDivElement))
    {
        element.innerText = arg.substring(0, 2) + ' : ' + arg.substring(2, 4) + ' : ' + arg.substring(4)
    }
}
function setQuotes(args)
{
    let se = document.getElementById('stock-quotes-grid-sell-price'),
        be = document.getElementById('stock-quotes-grid-buy-price')

    if (se instanceof (HTMLDivElement))
    {
        if (se.hasChildNodes())
        {
            for (var i = 0; i < se.children.length; i++)
            {
                se.children[i].firstChild.innerText = args[i].innerText
                se.children[i].lastChild.innerText = args[i].innerPercent
                se.children[i].style.color = args[i].color
            }
        }
    }
    if (be instanceof (HTMLDivElement))
    {
        if (be.hasChildNodes())
        {
            for (var i = 0; i < be.children.length; i++)
            {
                be.children[i].firstChild.innerText = args[10 + i].innerText
                be.children[i].lastChild.innerText = args[10 + i].innerPercent
                be.children[i].style.color = args[10 + i].color
            }
        }
    }
}