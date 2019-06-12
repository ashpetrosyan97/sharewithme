import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';


@Component({
    selector: 'app-used-space',
    templateUrl: './used-space.component.html',
    styleUrls: ['./used-space.component.scss']
})
export class UsedSpaceComponent implements OnInit, OnChanges {
    @Input() usedSpacePercent: number;
    public pieChartLabels: string[] = ['Used', 'Free'];
    public pieChartData: number[];
    public pieChartType: string = 'pie';
    public pieChartColors = [{backgroundColor: ['#e91e63', "#8fd9e2"]}];
    public options = {
        tooltips: {
            enabled: true,
            mode: 'single',
            callbacks: {
                label: (tooltipItem, data) => {
                    let allData = data.datasets[tooltipItem.datasetIndex].data;
                    let tooltipLabel = data.labels[tooltipItem.index];
                    let tooltipData = allData[tooltipItem.index];
                    return tooltipLabel + ": " + tooltipData + "%";
                }
            }
        }
    };

    ngOnInit(): void {
        this.pieChartData = [this.usedSpacePercent, 100 - this.usedSpacePercent]
    }

    public chartClicked(e: any): void {
        console.log(e);
    }

    public chartHovered(e: any): void {
        console.log(e);
    }

    ngOnChanges(changes: SimpleChanges): void {
        this.pieChartData = [this.usedSpacePercent, 100 - this.usedSpacePercent]
    }
}
