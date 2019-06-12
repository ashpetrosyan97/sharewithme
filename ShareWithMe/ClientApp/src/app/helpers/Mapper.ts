import {DeletedFileModel} from "@app/models/deletedFileModel";
import * as moment from "moment";

export class Mapper {
    static MapToFormData(model): FormData {
        let formData: FormData = new FormData();
        for (let key in model) {
            if (typeof model[key] === "object" && model[key].length)
                for (let i = 0; i < model[key].length; i++)
                    formData.append(key, model[key][i]);
            else
                formData.append(key, model[key]);
        }
        return formData;
    }

    static MapDeletedFileModel(list: Array<DeletedFileModel>) {
        let mappedArray = [];
        list.forEach((elm) => {
            mappedArray.push({
                id: elm.id,
                name: elm.name,
                deletionTime: moment(elm.deletionTime).format('DD MMMM YYYY, h:mm a'),
                creationTime: moment(elm.creationTime).format('DD MMMM YYYY, h:mm a'),
                lastModificationTime: moment(elm.lastModificationTime).format('DD MMMM YYYY, h:mm a'),
            })
        });
        return mappedArray;
    }
}
