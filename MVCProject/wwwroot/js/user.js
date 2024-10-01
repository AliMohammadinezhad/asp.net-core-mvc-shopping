let dataTable;

$(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable(
        {
            "ajax":
                { url: '/admin/user/getall' },
            "columns": [
                { data: 'name', "width": "25%" },
                { data: 'email', "width": "15%" },
                { data: 'phoneNumber', "width": "10%" },
                { data: 'company.name', "width": "15%" },
                { data: 'role', "width": "10%" },
                {
                    data: 'id',
                    "render": function (data) {
                        return `
                    <div class="w-75 btn-group" role="group">
                    <a href="/admin/user/getall?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Edit</a>
                    </div>
                    `
                    },
                    "width": "25%"
                },
            ]
        });
}


