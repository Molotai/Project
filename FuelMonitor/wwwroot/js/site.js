// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/jobshub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
});

start();

connection.on("ConcurrentJobs", function (message) {
    var li = document.createElement("li");
    document.getElementById("concurrentJobs").appendChild(li);
    li.textContent = `${message}`;
});

connection.on("NonConcurrentJobs", function (message) {
    var li = document.createElement("li");
    document.getElementById("nonConcurrentJobs").appendChild(li);
    li.textContent = `${message}`;
});

connection.on("TableContent", function (message, date) {
        var row = document.getElementById("table").rows;
        for (var i = 0; i < row.length; i++) {
            var col = row[i].cells;
            if (Number(col[0].innerText) == message.id) {
                col[2].innerHTML = message.name
                col[3].innerHTML = message.workDescription
                let position = date.search(":") + 6;
                date = date.replaceAll('T', ' ')
                col[4].innerHTML = date.substring(0, position);
            }
        }
})

