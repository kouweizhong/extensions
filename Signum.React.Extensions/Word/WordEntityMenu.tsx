﻿
import * as React from 'react'
import { RouteComponentProps } from 'react-router'
import { DropdownButton, MenuItem } from 'react-bootstrap'
import { Dic, classes } from '../../../Framework/Signum.React/Scripts/Globals'
import * as Finder from '../../../Framework/Signum.React/Scripts/Finder'
import { ResultTable, FindOptions, FilterOption, QueryDescription } from '../../../Framework/Signum.React/Scripts/FindOptions'
import { SearchMessage, JavascriptMessage, parseLite, is, Lite, toLite, Entity, EntityPack } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import SearchControlLoaded from '../../../Framework/Signum.React/Scripts/SearchControl/SearchControlLoaded'
import { WordTemplateEntity, WordTemplateMessage } from './Signum.Entities.Word'
import * as WordClient from './WordClient'
import { saveFile } from "../../../Framework/Signum.React/Scripts/Services";

export interface WordEntityMenuProps {
    entityPack: EntityPack<Entity>;
}

export default class WordEntityMenu extends React.Component<WordEntityMenuProps> {
    
    handleSelect = (wt: Lite<WordTemplateEntity>) => {

        Navigator.API.fetchAndForget(wt)
            .then<string | undefined>(wordTemplate => wordTemplate.systemWordTemplate ? WordClient.API.getConstructorType(wordTemplate.systemWordTemplate!) : undefined)
            .then(ct => {

                if (!ct)
                    return WordClient.API.createAndDownloadReport({ template: wt, lite: toLite(this.props.entityPack.entity) });

                var s = WordClient.settings[ct];
                if (!s)
                    throw new Error("No 'WordModelSettings' defined for '" + ct + "'");

                if (!s.createFromEntities)
                    throw new Error("No 'createFromEntities' defined in the WordModelSettings of '" + ct + "'");

                return s.createFromEntities(wt, [toLite(this.props.entityPack.entity)])
                    .then<Response | undefined>(m => m && WordClient.API.createAndDownloadReport({ template: wt, entity: m }));
            })
            .then(response => response && saveFile(response))
            .done();
    }

    render() {
        
        const label = <span><i className="fa fa-file-word-o"></i>&nbsp;{WordTemplateMessage.WordReport.niceToString()}</span>;

        return (
            <DropdownButton title={label as any} id="wordMenu" className="sf-word-dropdown">
                {
                    this.props.entityPack.wordTemplates!.map((wt, i) =>
                        <MenuItem key={i}
                            onSelect={() => this.handleSelect(wt)}>
                            {wt.toStr}
                        </MenuItem>)
                }
            </DropdownButton>
        );
    }
 
}



