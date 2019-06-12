import {notAllowedCharacters} from "../constants/NotAllowedCharacters";


export class Directory {
    public static ValidateFolderName(name: string): boolean {

        let isValid: boolean = true;
        notAllowedCharacters.forEach((char) => {
            if (name.includes(char)) isValid = false;
        });
        return isValid;
    }

    public static CreateNodes(list) {
        let map = {}, node, roots = [], i;
        for (i = 0; i < list.length; i += 1) {
            map[list[i].id] = i;
            if (list[i].type === 0)
                list[i].children = [];
        }
        for (i = 0; i < list.length; i += 1) {
            node = list[i];
            if (node.parentId !== 0) {
                list[map[node.parentId]].children.push(node);
            } else {
                roots.push(node);
            }
        }
        return roots;
    }

    public static getNodeById(tree, id) {
        let result = null;
        if (id === tree.id) {
            return tree;
        } else {
            if (tree.children)
                tree.children.some(node => result = this.getNodeById(node, id));

            return result;
        }
    };

    public static treeToArray(node, result) {
        result.push(node.id);
        if (node.children && node.children.length > 0) {
            for (let i = 0; i < node.children.length; i++) {
                this.treeToArray(node.children[i], result);
            }
            return result;
        }
    }

    public static findPath(node, array) {
        let path = 'Root';
        let pathArray = [];
        this.findAllParents(node, array, pathArray);
        pathArray.reverse().forEach(n => path = `${path}\\${n}`);
        return `${path}\\${node.name}`;
    }

    static findAllParents(node, array, result) {
        if (node.parentId !== 0) {
            let parentObj = array.find(x => x.id === node.parentId);
            result.push(parentObj.name);
            this.findAllParents(parentObj, array, result);
        } else {
            return;
        }
    }


    public static filterDirectories(array, id) {
        let result = [];
        let obj = {id: 0, name: 'Root', children: []};
        obj.id = 0;
        obj.children = [...this.CreateNodes(array)];
        if (id) {
            let node = this.getNodeById(obj, id);
            this.treeToArray(node, result);
            result.push(node.id);
        }

        let final = array.filter(x => !result.includes(x.id))
            .filter(x => x.type === 0);
        final.forEach(x => x.path = this.findPath(this.getNodeById(obj, x.id), array));
        return [{id: 0, name: 'Root', type: 0, children: this.CreateNodes(final)}];
    }

}
