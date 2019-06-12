import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
    name: 'duration'
})
export class DurationPipe implements PipeTransform {

    transform(value: string, args?: any): any {
        return value.substring(0, value.indexOf('.'));
    }

}
