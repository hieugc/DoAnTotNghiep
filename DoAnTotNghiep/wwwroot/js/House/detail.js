function LoadMap(lat, lng) {
    let loc = new Microsoft.Maps.Location(lat, lng);
    var map = new Microsoft.Maps.Map('#myMap', {
        zoom: 15,
        center: loc
    });
    let pin = new Microsoft.Maps.Pushpin(loc, {
        title: '@Html.Raw(Model.Name)',
        subTitle: '@Html.Raw(Model.Location)'
    });
    map.entities.push(pin);
}