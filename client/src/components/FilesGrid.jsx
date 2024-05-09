import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { Button } from "primereact/button";

function FileList({ files, onDelete }) {
    const actionBodyTemplate = (rowData) => {
        return (
            <Button
                icon="pi pi-trash"
                rounded
                outlined
                severity="danger"
                onClick={() => onDelete(rowData.id)}
            />
        );
    };

    const dateBodyTemplate = (rowData) => {
        const date = new Date(rowData.createdDateUtc);

        return date.toLocaleDateString("en-US", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
        });
    };

    const sizeBodyTemplate = (rowData) => {
        const sizeInMbs = rowData.contentLength / (1024 * 1024);

        return `${sizeInMbs.toFixed(2)} MBs`;
    };

    return (
        <div className="grid">
            <DataTable value={files[0]} tableStyle={{ minWidth: "60rem", maxWidth: "60rem" }}>
                <Column field="fullFileName" header="Name"></Column>
                <Column field="contentLength" header="Size" body={sizeBodyTemplate}></Column>
                <Column
                    field="createdDateUtc"
                    header="Date"
                    dataType="date"
                    body={dateBodyTemplate}
                ></Column>
                <Column
                    body={actionBodyTemplate}
                    header="Actions"
                    exportable={false}
                    style={{ minWidth: "12rem" }}
                ></Column>
            </DataTable>
        </div>
    );
}

export default FileList;
