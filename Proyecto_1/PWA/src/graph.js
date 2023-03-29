const drawLine = (ctx, startX, startY, endX, endY,color)=>{
    ctx.save();
    ctx.strokeStyle = color;
    ctx.beginPath();
    ctx.moveTo(startX,startY);
    ctx.lineTo(endX,endY);
    ctx.stroke();
    ctx.restore();
}
const drawBar = (ctx, upperLeftCornerX, upperLeftCornerY, width, height,color)=>{
    ctx.save();
    ctx.fillStyle=color;
    ctx.fillRect(upperLeftCornerX,upperLeftCornerY,width,height);
    ctx.restore();
}
class Barchart {
    constructor(options) {
        this.options = options;
        this.canvas = options.canvas;
        this.ctx = this.canvas.getContext("2d");
        this.colors = options.colors;
        this.canvas.height = 400
        this.canvas.width = this.canvas.parentElement.clientWidth

        this.draw = function () {
            let maxValue = 100;
            let canvasActualHeight = this.canvas.height - this.options.padding * 2;
            let canvasActualWidth = this.canvas.width - this.options.padding * 2;
            let gridValue = 0;
            while (gridValue <= maxValue) {
                let gridY = canvasActualHeight * (1 - gridValue / maxValue) + this.options.padding;
                drawLine(this.ctx,0,gridY,this.canvas.width,gridY,this.options.gridColor);

                this.ctx.save();
                this.ctx.fillStyle = this.options.gridColor;
                this.ctx.textBaseline = "bottom";
                this.ctx.font = "bold 10px Arial";
                this.ctx.fillText(gridValue, 10, gridY - 2);
                this.ctx.restore();
                gridValue += this.options.gridScale;
            }

            //drawing the bars 
            let barIndex = 0;
            var numberOfBars = Object.keys(this.options.data).length;
            var barSize = (canvasActualWidth) / numberOfBars;
            for (let categ in this.options.data) {
                var val = this.options.data[categ];
                var barHeight = Math.round(canvasActualHeight * val / maxValue);
                drawBar(
                    this.ctx,
                    this.options.padding + barIndex * barSize,
                    this.canvas.height - barHeight - this.options.padding,
                    barSize,
                    barHeight,
                    this.colors[barIndex % this.colors.length]
                );
                barIndex++;
            }
            //drawing series name 
            this.ctx.save();
            this.ctx.textBaseline = "bottom";
            this.ctx.textAlign = "center";
            this.ctx.fillStyle = "#000000";
            this.ctx.font = "bold 14px Arial";
            this.ctx.fillText(this.options.seriesName, this.canvas.width / 2, this.canvas.height);
            this.ctx.restore();

            //draw legend 
            barIndex = 0;
            let legend = document.querySelector(`legend[for='${this.canvas.id}']`);
            legend.innerHTML=""
            let ul = document.createElement("ul");
            legend.append(ul);
            for (let categ in this.options.data) {
                let li = document.createElement("li");
                li.style.listStyle = "none";
                li.style.borderLeft = "20px solid " + this.colors[barIndex % this.colors.length];
                li.style.padding = "5px";
                li.textContent = categ;
                ul.append(li);
                barIndex++;
            }
        };
    }
}
class Barchart2 {
    constructor(options) {
        this.options = options;
        this.canvas = options.canvas;
        this.ctx = this.canvas.getContext("2d");
        this.colors = options.colors;
        this.canvas.height = 400
        this.canvas.width = this.canvas.parentElement.clientWidth

        this.draw = function () {
            let maxValue = 2;
            let canvasActualHeight = this.canvas.height - this.options.padding * 2;
            let canvasActualWidth = this.canvas.width - this.options.padding * 2;
            let gridValue = 0;
            while (gridValue <= maxValue) {
                let gridY = canvasActualHeight * (1 - gridValue / maxValue) + this.options.padding;
                drawLine(this.ctx,0,gridY,this.canvas.width,gridY,this.options.gridColor);
                this.ctx.save();
                this.ctx.fillStyle = this.options.gridColor;
                this.ctx.textBaseline = "bottom";
                this.ctx.font = "bold 10px Arial";
                this.ctx.fillText(gridY>=300?'Sentado':'Parado', 10, gridY - 2);
                this.ctx.restore();
                gridValue += this.options.gridScale;
            }

            //drawing the bars 
            let barIndex = 0;
            var numberOfBars = Object.keys(this.options.data).length;
            var barSize = (canvasActualWidth) / numberOfBars;
            for (let categ in this.options.data) {
                var val = this.options.data[categ];
                var barHeight = Math.round(canvasActualHeight * val / maxValue);
                drawBar(
                    this.ctx,
                    this.options.padding + barIndex * barSize,
                    this.canvas.height - barHeight - this.options.padding,
                    barSize,
                    barHeight,
                    this.colors[barIndex % this.colors.length]
                );
                barIndex++;
            }
            //drawing series name 
            this.ctx.save();
            this.ctx.textBaseline = "bottom";
            this.ctx.textAlign = "center";
            this.ctx.fillStyle = "#000000";
            this.ctx.font = "bold 14px Arial";
            this.ctx.fillText(this.options.seriesName, this.canvas.width / 2, this.canvas.height);
            this.ctx.restore();

            //draw legend 
            barIndex = 0;
            let legend = document.querySelector(`legend[for='${this.canvas.id}']`);
            legend.innerHTML=""
            let ul = document.createElement("ul");
            legend.append(ul);
            for (let categ in this.options.data) {
                let li = document.createElement("li");
                li.style.listStyle = "none";
                li.style.borderLeft = "20px solid " + this.colors[barIndex % this.colors.length];
                li.style.padding = "5px";
                li.textContent = categ;
                ul.append(li);
                barIndex++;
            }
        };
    }
}

class Barchart3 {
    constructor(options) {
        this.options = options;
        this.canvas = options.canvas;
        this.ctx = this.canvas.getContext("2d");
        this.colors = options.colors;
        this.canvas.height = 400
        this.canvas.width = this.canvas.parentElement.clientWidth

        this.draw = function () {
            let maxValue = 2;
            let canvasActualHeight = this.canvas.height - this.options.padding * 2;
            let canvasActualWidth = this.canvas.width - this.options.padding * 2;
            let gridValue = 0;
            while (gridValue <= maxValue) {
                let gridY = canvasActualHeight * (1 - gridValue / maxValue) + this.options.padding;
                drawLine(this.ctx,0,gridY,this.canvas.width,gridY,this.options.gridColor);
                this.ctx.save();
                this.ctx.fillStyle = this.options.gridColor;
                this.ctx.textBaseline = "bottom";
                this.ctx.font = "bold 10px Arial";
                this.ctx.fillText(gridY>=300?'Parado':'Sentado', 10, gridY - 2);
                this.ctx.restore();
                gridValue += this.options.gridScale;
            }

            //drawing the bars 
            let barIndex = 0;
            var numberOfBars = Object.keys(this.options.data).length;
            var barSize = (canvasActualWidth) / numberOfBars;
            for (let categ in this.options.data) {
                var val = this.options.data[categ];
                var barHeight = Math.round(canvasActualHeight * val / maxValue);
                drawBar(
                    this.ctx,
                    this.options.padding + barIndex * barSize,
                    this.canvas.height - barHeight - this.options.padding,
                    barSize,
                    barHeight,
                    this.colors[barIndex % this.colors.length]
                );
                barIndex++;
            }
            //drawing series name 
            this.ctx.save();
            this.ctx.textBaseline = "bottom";
            this.ctx.textAlign = "center";
            this.ctx.fillStyle = "#000000";
            this.ctx.font = "bold 14px Arial";
            this.ctx.fillText(this.options.seriesName, this.canvas.width / 2, this.canvas.height);
            this.ctx.restore();

            //draw legend 
            barIndex = 0;
            let legend = document.querySelector(`legend[for='${this.canvas.id}']`);
            legend.innerHTML=""
            let ul = document.createElement("ul");
            legend.append(ul);
            for (let categ in this.options.data) {
                let li = document.createElement("li");
                li.style.listStyle = "none";
                li.style.borderLeft = "20px solid " + this.colors[barIndex % this.colors.length];
                li.style.padding = "5px";
                li.textContent = categ;
                ul.append(li);
                barIndex++;
            }
        };
    }
}

class Barchart4 {
    constructor(options) {
        this.options = options;
        this.canvas = options.canvas;
        this.ctx = this.canvas.getContext("2d");
        this.colors = options.colors;
        this.canvas.height = 400
        this.canvas.width = this.canvas.parentElement.clientWidth

        this.draw = function () {
            let maxValue = 8;
            let canvasActualHeight = this.canvas.height - this.options.padding * 2;
            let canvasActualWidth = this.canvas.width - this.options.padding * 2;
            let gridValue = 0;
            while (gridValue <= maxValue) {
                let gridY = canvasActualHeight * (1 - gridValue / maxValue) + this.options.padding;
                drawLine(this.ctx,0,gridY,this.canvas.width,gridY,this.options.gridColor);

                this.ctx.save();
                this.ctx.fillStyle = this.options.gridColor;
                this.ctx.textBaseline = "bottom";
                this.ctx.font = "bold 10px Arial";
                this.ctx.fillText(gridValue, 10, gridY - 2);
                this.ctx.restore();
                gridValue += this.options.gridScale;
            }

            //drawing the bars 
            let barIndex = 0;
            var numberOfBars = Object.keys(this.options.data).length;
            var barSize = (canvasActualWidth) / numberOfBars;
            for (let categ in this.options.data) {
                var val = this.options.data[categ];
                var barHeight = Math.round(canvasActualHeight * val / maxValue);
                drawBar(
                    this.ctx,
                    this.options.padding + barIndex * barSize,
                    this.canvas.height - barHeight - this.options.padding,
                    barSize,
                    barHeight,
                    this.colors[barIndex % this.colors.length]
                );
                barIndex++;
            }
            //drawing series name 
            this.ctx.save();
            this.ctx.textBaseline = "bottom";
            this.ctx.textAlign = "center";
            this.ctx.fillStyle = "#000000";
            this.ctx.font = "bold 14px Arial";
            this.ctx.fillText(this.options.seriesName, this.canvas.width / 2, this.canvas.height);
            this.ctx.restore();

            //draw legend 
            barIndex = 0;
            let legend = document.querySelector(`legend[for='${this.canvas.id}']`);
            legend.innerHTML=""
            let ul = document.createElement("ul");
            legend.append(ul);
            for (let categ in this.options.data) {
                let li = document.createElement("li");
                li.style.listStyle = "none";
                li.style.borderLeft = "20px solid " + this.colors[barIndex % this.colors.length];
                li.style.padding = "5px";
                li.textContent = categ;
                ul.append(li);
                barIndex++;
            }
        };
    }
}

/*---------------------------------------------------------------*/
let livegraph = document.getElementById("live");
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
let usuario1 = document.getElementById("usuario");
let fechaInicio1 = document.getElementById("fechainicio");
let fechafin1 = document.getElementById("fechafin");
let btnbuscar1 = document.getElementById("btnbuscar");
let selecthistorica = document.getElementById("opcionhistorica");
let historicagraph = document.getElementById("historica");
let hisotorticodata = []
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
let usuariop1 = document.getElementById("usuariop1");
let fechaIniciop1 = document.getElementById("fechainiciop1");
let fechafinp1 = document.getElementById("fechafinp1");
let btnbuscarp1 = document.getElementById("btnbuscarp1");
let selectp1 = document.getElementById("opcionp1");
let p1 = document.getElementById("p1");
let p1data = []
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
let usuariod1 = document.getElementById("usuariod1");
let fechaIniciod1 = document.getElementById("fechainiciod1");
let fechafind1 = document.getElementById("fechafind1");
let btnbuscard1 = document.getElementById("btnbuscard1");
let selectd1 = document.getElementById("opciond1");
let d1 = document.getElementById("d1");
let d1data = []
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
let usuariop2 = document.getElementById("usuariop2");
let fechaIniciop2 = document.getElementById("fechainiciop2");
let fechafinp2 = document.getElementById("fechafinp2");
let btnbuscarp2 = document.getElementById("btnbuscarp2");
let selectp2 = document.getElementById("opcionp2");
let p2 = document.getElementById("p2");
let p2data = []
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
let usuariod2 = document.getElementById("usuariod2");
let fechaIniciod2 = document.getElementById("fechainiciod2");
let fechafind2 = document.getElementById("fechafind2");
let btnbuscard2 = document.getElementById("btnbuscard2");
let selectd2 = document.getElementById("opciond2");
let d2 = document.getElementById("d2");
let d2data = []
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
let usuariopenalizacion = document.getElementById("usuariopenalizacion");
let fechaIniciopenalizacion = document.getElementById("fechainiciopenalizacion");
let fechafinpenalizacion = document.getElementById("fechafinpenalizacion");
let btnbuscarpenalizacion = document.getElementById("btnbuscarpenalizacion");
let penalizacion = document.getElementById("penalizacion");
let penalizaciondata = []
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/

let prueba = 0;
let pruebaLive;
const ejecutarlive = async ()=>{
    let data = null;
    // await fetch('http://localhost:3000/data')
    await fetch(`http://localhost:5000/tiempoReal`, {
        method: 'POST',
        body: JSON.stringify({"nameUser": sessionStorage.getItem('user-name')}),
        headers: {
            'Content-type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(d => pruebaLive = d);
    data = pruebaLive[0]
    let stringdata = "{\n"
    let values = Object.keys(data);
    for(let i = 7;i<Object.keys(data).length;i++){
        if (values[i].includes("p")) console.log(values[i]);
        if(values[i].includes("p") && data[values[i]] != null){
            stringdata+=`"${i-6+". "+data["fecha_corta"]+" "+values[i]+" "+data[values[i]].inicio.substring(10, data[values[i]].inicio.length)+"-"+data[values[i]].fin.substring(10, data[values[i]].fin.length)+" "+100*data[values[i]].tiempo/data["tiempoStandar"]}":${100*data[values[i]].tiempo/data["tiempoStandar"]},\n`
        }else if (values[i].includes("d") && parseInt(values[i][1]) >= 0 && data[values[i]] != null){
            stringdata+=`"${i-6+". "+data["fecha_corta"]+" "+values[i]+" "+data[values[i]].inicio.substring(10, data[values[i]].inicio.length)+"-"+data[values[i]].fin.substring(10, data[values[i]].fin.length)+" "+100*data[values[i]].tiempo/data["descansoStandar"]}":${100*data[values[i]].tiempo/data["descansoStandar"]},\n`
        }
        if(values[i].includes("d") && parseInt(values[i][1]) >= 0) console.log(values[i]);
    }
    stringdata=stringdata.substring(0, stringdata.length - 2);
    stringdata+="}"
    console.log(stringdata)
    let datalive = JSON.parse(stringdata)
    let myBarchart = new Barchart(
        {
            canvas:livegraph,
            seriesName:"Tabla En VIVO",
            padding:25,
            gridScale:25,
            gridColor:"#a7a5a5",
            data:datalive,
            colors:["#a55ca5","#67b6c7"]
        }
    );
    myBarchart.draw()
}
ejecutarlive();
setInterval(ejecutarlive, 8000);
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
// let pruebasss;
const ejecutarhisotrica = (datainput)=>{
    let data = datainput;
    let stringdata = "{\n"
    let values = Object.keys(data)
    console.log(values);
    // pruebasss = values;
    for(let i = 7;i<values.length;i++){
        if(values[i].includes("p")){
            stringdata+=`"${i-6+". "+data["fecha"]+" "+values[i]+" "+data[values[i]].inicio.substring(10, data[values[i]].inicio.length)+"-"+data[values[i]].fin.substring(10, data[values[i]].fin.length)+" "+100*data[values[i]].tiempo/data["tiempoStandar"]}":${100*data[values[i]].tiempo/data["tiempoStandar"]},\n`
        }else if (values[i].includes("d") && parseInt(values[i][1]) >= 0){
            console.log(values[i]);
            stringdata+=`"${i-6+". "+data["fecha"]+" "+values[i]+" "+data[values[i]].inicio.substring(10, data[values[i]].inicio.length)+"-"+data[values[i]].fin.substring(10, data[values[i]].fin.length)+" "+100*data[values[i]].tiempo/data["descansoStandar"]}":${100*data[values[i]].tiempo/data["descansoStandar"]},\n`
        }
    }
    stringdata=stringdata.substring(0, stringdata.length - 2);
    stringdata+="}"
    let datalive = JSON.parse(stringdata)
    let myBarchart = new Barchart(
        {
            canvas:historicagraph,
            seriesName:"Tabla Historica",
            padding:25,
            gridScale:25,
            gridColor:"#a7a5a5",
            data:datalive,
            colors:["#a55ca5","#67b6c7"]
        }
    );
    myBarchart.draw()
    
}
btnbuscar1.addEventListener("click", async function(e){
    e.preventDefault()
    let fechainiciof = getFecha(fechaInicio1.value)
    let fechafinf = getFecha(fechafin1.value)
    let usuariof = usuario1.value

    let datos = {
        "nameUser": sessionStorage.getItem('user-name'),
        "fecha1": fechainiciof,
        "fecha2": fechafinf
    }
    
    await fetch('http://localhost:5000/grafica1', {
        method: 'POST',
        body: JSON.stringify(datos),
        headers: {
            'Content-type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(d => hisotorticodata = d);
    let data = hisotorticodata
    console.log(data)
    let etiquetas=[];
    for(let i in data){
        etiquetas.push(data[i].fecha)
    }
    selecthistorica.innerHTML="";
    for (let categ of etiquetas) {
        let li = document.createElement("option");
        li.value=categ
        li.textContent = categ;
        selecthistorica.append(li);
    }
}, false);

selecthistorica.addEventListener("change", function(e){
    e.preventDefault()
    selecthistorica.options[selecthistorica.selectedIndex].text
    let dato=null
    for(let categ of hisotorticodata){
        if(selecthistorica.options[selecthistorica.selectedIndex].text===categ.fecha){
            dato=categ
        }
    }
    if(dato!=null){
        ejecutarhisotrica(dato)
    }
})
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
const ejecutarp1 = (datainput)=>{
    let data = datainput;
    let stringdata = "{\n"
    let pomodoros = data.trabajo
    for(let categ of pomodoros){
        for(let i of categ.dataPrimaria){
            for(let j of i.arreglo){
                stringdata+=`"${i.nameElemento+" "+j.clave}":${j.valor==1?1:0.1},\n`
            }
        }
    }
    stringdata=stringdata.substring(0, stringdata.length - 2);
    stringdata+="}"
    console.log(stringdata)
    let datalive = JSON.parse(stringdata)
    let myBarchart = new Barchart2(
        {
            canvas:p1,
            seriesName:"Penalizacion Parado",
            padding:100,
            gridScale:1,
            gridColor:"#a7a5a5",
            data:datalive,
            colors:["#a55ca5"]
        }
    );
    myBarchart.draw()
}

function createOptNotSelected(selectElem){
    let li = document.createElement("option");
    li.value="None";
    li.textContent = "No seleccionado";
    selectElem.append(li);
}

btnbuscarp1.addEventListener("click", async function(e){
    e.preventDefault()
    let fechainiciof = getFecha(fechaIniciop1.value);
    let fechafinf = getFecha(fechafinp1.value);
    let usuariof = usuariop1.value

    let datos = {
        "nameUser": sessionStorage.getItem('user-name'),
        "fecha1": fechainiciof,
        "fecha2": fechafinf
    }
    console.log(datos);
    await fetch('http://localhost:5000/grafica_4_5_6', {
        method: 'POST',
        body: JSON.stringify(datos),
        headers: {
        'Content-type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(d => p1data = d);
    console.log(p1data);
    let data = p1data
    let auxXD = data['data'];
    let etiquetas=[];
    for(let i in auxXD){
        etiquetas.push(auxXD[i].fecha)
    }
    selectp1.innerHTML="";

    createOptNotSelected(selectp1);

    for (let categ of etiquetas) {
        let li = document.createElement("option");
        li.value=categ
        li.textContent = categ;
        selectp1.append(li);
    }
}, false);
selectp1.addEventListener("change", function(e){
    e.preventDefault()
    selectp1.options[selectp1.selectedIndex].text
    let dato=null
    for(let categ of p1data['data']){
        if(selectp1.options[selectp1.selectedIndex].text===categ.fecha){
            dato=categ
        }
    }
    if(dato!=null){
        ejecutarp1(dato)
    }
})
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
const ejecutard1 = (datainput)=>{
    let data = datainput;
    let stringdata = "{\n"
    let pomodoros = data.descansos
    for(let categ of pomodoros){
        for(let i of categ.dataPrimaria){
            for(let j of i.arreglo){
                stringdata+=`"${i.nameElemento+" "+j.clave}":${j.valor==1?1:0.1},\n`
            }
        }
    }
    
    stringdata=stringdata.substring(0, stringdata.length - 2);
    stringdata+="}"
    console.log(stringdata)
    let datalive = JSON.parse(stringdata)
    let myBarchart = new Barchart2(
        {
            canvas:d1,
            seriesName:"Penalizacion centado",
            padding:100,
            gridScale:1,
            gridColor:"#a7a5a5",
            data:datalive,
            colors:["#9FE2BF","#6495ED"]
        }
    );
    myBarchart.draw()
    
}
btnbuscard1.addEventListener("click", async function(e){
    e.preventDefault()
    let fechainiciof = getFecha(fechaIniciop1.value)
    let fechafinf = getFecha(fechafinp1.value);
    let usuariof = usuariop1.value
    
    let datos = {
        "nameUser": sessionStorage.getItem('user-name'),
        "fecha1": fechainiciof,
        "fecha2": fechafinf
    }
    console.log(datos);
    await fetch('http://localhost:5000/grafica_4_5_6', {
        method: 'POST',
        body: JSON.stringify(datos),
        headers: {
        'Content-type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(d => d1data = d);
    let data = d1data
    let auxXD = data['data'];
    let etiquetas=[];
    for(let i in auxXD){
        etiquetas.push(auxXD[i].fecha)
    }
    selectd1.innerHTML="";

    createOptNotSelected(selectd1);

    for (let categ of etiquetas) {
        let li = document.createElement("option");
        li.value=categ
        li.textContent = categ;
        selectd1.append(li);
    }
}, false);

selectd1.addEventListener("change", function(e){
    e.preventDefault()
    selectd1.options[selectd1.selectedIndex].text
    let dato=null
    for(let categ of d1data['data']){
        if(selectd1.options[selectd1.selectedIndex].text===categ.fecha){
            dato=categ
        }
    }
    if(dato!=null){
        ejecutard1(dato)
    }
})
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
const ejecutarp2 = (datainput)=>{
    let data = datainput;
    let stringdata = "{\n"
    let pomodoros = data.trabajo
    for(let categ of pomodoros){
        for(let i of categ.dataPrimaria){
            for(let j of i.arreglo){
                stringdata+=`"${i.nameElemento+" "+j.clave}":${j.valor==1?0.1:1},\n`
            }
        }
    }
    
    stringdata=stringdata.substring(0, stringdata.length - 2);
    stringdata+="}"
    console.log(stringdata)
    let datalive = JSON.parse(stringdata)
    let myBarchart = new Barchart3(
        {
            canvas:p2,
            seriesName:"Penalizacion Parado Inversa",
            padding:100,
            gridScale:1,
            gridColor:"#a7a5a5",
            data:datalive,
            colors:["#a55ca5"]
        }
    );
    myBarchart.draw()
    
}
btnbuscarp2.addEventListener("click", async function(e){
    e.preventDefault()
    let fechainiciof = getFecha(fechaIniciop2.value);
    let fechafinf = getFecha(fechafinp2.value);
    let usuariof = usuariop2.value
    
    let datos = {
        "nameUser": sessionStorage.getItem('user-name'),
        "fecha1": fechainiciof,
        "fecha2": fechafinf
    }
    console.log(datos);
    await fetch('http://localhost:5000/grafica_4_5_6', {
        method: 'POST',
        body: JSON.stringify(datos),
        headers: {
        'Content-type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(d => p2data = d);
    let data = p2data
    let auxXD = data['data']
    let etiquetas=[];
    for(let i in auxXD){
        etiquetas.push(auxXD[i].fecha)
    }
    selectp2.innerHTML="";

    createOptNotSelected(selectp2);

    for (let categ of etiquetas) {
        let li = document.createElement("option");
        li.value=categ
        li.textContent = categ;
        selectp2.append(li);
    }
}, false);
selectp2.addEventListener("change", function(e){
    e.preventDefault()
    selectp2.options[selectp2.selectedIndex].text
    let dato=null
    for(let categ of p2data['data']){
        if(selectp2.options[selectp2.selectedIndex].text===categ.fecha){
            dato=categ
        }
    }
    if(dato!=null){
        ejecutarp2(dato)
    }
})
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
const ejecutard2 = (datainput)=>{
    let data = datainput;
    let stringdata = "{\n"
    let pomodoros = data.descansos
    for(let categ of pomodoros){
        for(let i of categ.dataPrimaria){
            for(let j of i.arreglo){
                stringdata+=`"${i.nameElemento+" "+j.clave}":${j.valor==1?0.1:1},\n`
            }
        }
    }
    
    stringdata=stringdata.substring(0, stringdata.length - 2);
    stringdata+="}"
    console.log(stringdata)
    let datalive = JSON.parse(stringdata)
    let myBarchart = new Barchart3(
        {
            canvas:d2,
            seriesName:"Penalizacion centado Inverso",
            padding:100,
            gridScale:1,
            gridColor:"#a7a5a5",
            data:datalive,
            colors:["#a55ca5"]
        }
    );
    myBarchart.draw()
    
}
btnbuscard2.addEventListener("click", async function(e){
    e.preventDefault()
    let fechainiciof = getFecha(fechaIniciod2.value);
    let fechafinf = getFecha(fechafind2.value);
    let usuariof = usuariod2.value
    
    let datos = {
        "nameUser": sessionStorage.getItem('user-name'),
        "fecha1": fechainiciof,
        "fecha2": fechafinf
    }
    console.log(datos);
    await fetch('http://localhost:5000/grafica_4_5_6', {
        method: 'POST',
        body: JSON.stringify(datos),
        headers: {
        'Content-type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(d => d2data = d);
    let data = d2data
    let auxXD = data['data'];
    let etiquetas=[];
    for(let i in auxXD){
        etiquetas.push(auxXD[i].fecha)
    }
    selectd2.innerHTML="";

    createOptNotSelected(selectd2);

    for (let categ of etiquetas) {
        let li = document.createElement("option");
        li.value=categ
        li.textContent = categ;
        selectd2.append(li);
    }
}, false);
selectd2.addEventListener("change", function(e){
    e.preventDefault()
    selectd2.options[selectd2.selectedIndex].text
    let dato=null
    for(let categ of d2data['data']){
        if(selectd2.options[selectd2.selectedIndex].text===categ.fecha){
            dato=categ
        }
    }
    if(dato!=null){
        ejecutard2(dato)
    }
})
/*---------------------------------------------------------------*/
/*---------------------------------------------------------------*/
const ejecutarpenalizacion = (datainput)=>{
    let data = datainput;
    let stringdata = "{\n"
    let fechas = []
    for(let e of data){
        if(!fechas.includes(e.fecha_corta)){
            fechas.push(e.fecha_corta)
        }
    }
    for(let e of fechas){
        let aux = data.filter(word => word.fecha_corta == e)
        let nopomodoros = 0;
        let tiempoPomodoro = 0;
        let pomodorospem = 0;
        let descansospen = 0;
        for(let i of aux){
            if(i.p1!=undefined){
                nopomodoros++;
                tiempoPomodoro+=i.tiempoStandar
                pomodorospem+=i.p1.penalizacion
            }
            if(i.p2!=undefined){
                nopomodoros++;
                tiempoPomodoro+=i.tiempoStandar
                pomodorospem+=i.p2.penalizacion
            }
            if(i.p3!=undefined){
                nopomodoros++;
                tiempoPomodoro+=i.tiempoStandar
                pomodorospem+=i.p3.penalizacion
            }
            if(i.p4!=undefined){
                nopomodoros++;
                tiempoPomodoro+=i.tiempoStandar
                pomodorospem+=i.p4.penalizacion
            }   
            if(i.d1!=undefined){
                descansospen+=i.d1.penalizacion
            }
            if(i.d2!=undefined){
                descansospen+=i.d2.penalizacion
            }
            if(i.d3!=undefined){
                descansospen+=i.d3.penalizacion
            }
            if(i.d4!=undefined){
                descansospen+=i.d4.penalizacion
            }
        }
        stringdata += `"${"CantPomodoros "+nopomodoros+" Fecha "+e}":${tiempoPomodoro/60},\n`
        stringdata += `"${"PenPomodoros"+" Fecha "+e}":${pomodorospem/60},\n`
        stringdata += `"${"PenDescansos"+" Fecha "+e}":${descansospen/60},\n`
    }
    stringdata=stringdata.substring(0, stringdata.length - 2);
    stringdata+="}"
    console.log(stringdata)
    let datalive = JSON.parse(stringdata)
    let myBarchart = new Barchart4(
        {
            canvas:penalizacion,
            seriesName:"Historico Penalizacion",
            padding:20,
            gridScale:1,
            gridColor:"#a7a5a5",
            data:datalive,
            colors:["#FFC300","#FF5733","#C70039"]
        }
    );
    myBarchart.draw()
    
}

function getFecha(fecha){
    let arrayFecha = fecha.split("-");
    return arrayFecha[2] + "/" + arrayFecha[1] + "/" + arrayFecha[0];
}

btnbuscarpenalizacion.addEventListener("click", async function(e){
    e.preventDefault()
    let fechainiciof = getFecha(fechaIniciopenalizacion.value)
    let fechafinf = getFecha(fechafinpenalizacion.value)
    let usuariof = usuariopenalizacion.value


    let datos = {
        "nameUser": sessionStorage.getItem('user-name'),
        "fecha1": fechainiciof,
        "fecha2": fechafinf
    }
    
    await fetch('http://localhost:5000/grafica2', {
        method: 'POST',
        body: JSON.stringify(datos),
        headers: {
            'Content-type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(d => penalizaciondata = d);
    let data = penalizaciondata
    console.log(data)
    ejecutarpenalizacion(data)

}, false);