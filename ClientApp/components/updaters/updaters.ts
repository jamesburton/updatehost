import * as ko from 'knockout';
import fetch from 'isomorphic-fetch';

interface IUpdaterProject {
    name: string;
}

const loadProjectsUrl = "/projects";
class UpdatersViewModel {
    projects: KnockoutObservableArray<IUpdaterProject>;
    loaded: KnockoutObservable<boolean>;
    loadProjects: () => void;
    loadedProjects: (projects:IUpdaterProject[]) => void;
    constructor() {
        //this.projects = ko.observableArray([{name: 'WiFiLED'}, {name: 'Example Project'}]);
        this.projects = ko.observableArray([]);
        this.loaded = ko.observable(false);

        this.loadedProjects = (projects:IUpdaterProject[]) => { this.projects(projects); };
        this.loadProjects = () =>  {
            alert('About to load projects');
            this.loaded(false);
            this.projects([]);
            fetch(loadProjectsUrl)
                .then(res => res.json())
                .then(data => {
                    console.log('Projects loaded: data=', data);
                    return data;
                })
                .then(data => this.loadedProjects(data))
                ;
        };

        window.setTimeout(0, () => this.loadProjects());
    }
}

export default { viewModel: UpdatersViewModel, template: require('./updaters.html') };
