module Colors

let private colors = [
  "#EB5545";
  "#F9D84A";
  "#68CE6A";
  "#89C1FA";
  "#5E5CDE";
  "#EC5D7B";
  "#C983EE";
  "#C3A77C";
  "#747E86";
  "#E3B7B0"
]

let getRandomColor () =
  let rnd = System.Random()
  colors[rnd.Next(0, colors.Length)]
