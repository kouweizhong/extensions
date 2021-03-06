﻿import * as React from 'react'
import { RouteComponentProps } from 'react-router-dom'
import * as numbro from 'numbro'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import * as Finder from '../../../Framework/Signum.React/Scripts/Finder'
import EntityLink from '../../../Framework/Signum.React/Scripts/SearchControl/EntityLink'
import {ValueSearchControl, SearchControl } from '../../../Framework/Signum.React/Scripts/Search'
import { QueryDescription, SubTokensOptions } from '../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName, PropertyRoute, getTypeInfos } from '../../../Framework/Signum.React/Scripts/Reflection'
import { ModifiableEntity, EntityControlMessage, Entity, parseLite, getToString, JavascriptMessage } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import { API, ProcessLogicState } from './ProcessClient'
import { ProcessEntity } from './Signum.Entities.Processes'



interface ProcessPanelProps extends RouteComponentProps<{}> {

}

export default class ProcessPanelPage extends React.Component<ProcessPanelProps, ProcessLogicState> {

    componentWillMount() {
        this.loadState().done();

        Navigator.setTitle("ProcessLogic state");
    }

    componentWillUnmount() {
        Navigator.setTitle();
    }

    loadState() {
        return API.view()
            .then(s => this.setState(s));
    }

    handleStop = (e: React.MouseEvent<any>) => {
        e.preventDefault();
        API.stop().then(() => this.loadState()).done();
    }

    handleStart = (e: React.MouseEvent<any>) => {
        e.preventDefault();
        API.start().then(() => this.loadState()).done();
    }


    render() {

        if (this.state == undefined)
            return <h2>ProcesLogic state (loading...) </h2>;

        const s = this.state;

        return (
            <div>
                <h2>ProcessLogic state</h2>
                <div className="btn-toolbar">
                    {s.Running && <a href="" className="sf-button btn btn-default active" style={{ color: "red" }} onClick={this.handleStop}>Stop</a> }
                    {!s.Running && <a href="" className="sf-button btn btn-default" style={{ color: "green" }} onClick={this.handleStart}>Start</a> }
                </div >
                <div id="processMainDiv">
                    <br />
                    State: <strong>
                        {s.Running ?
                            <span style={{ color: "Green" }}> RUNNING </span> :
                            <span style={{ color: "Red" }}> STOPPED </span>
                        }</strong>
                    <br />
                    JustMyProcesses: {s.JustMyProcesses.toString()}
                    <br />
                    MaxDegreeOfParallelism: { s.MaxDegreeOfParallelism}
                    <br />
                    InitialDelayMiliseconds: { s.InitialDelayMiliseconds }
                    <br />
                    NextPlannedExecution: { s.NextPlannedExecution || "-None-" }
                    <br />
                    <table className="table">
                        <thead>
                            <tr>
                                <th>Process
                                </th>
                                <th>State
                                </th>
                                <th>Progress
                                </th>
                                <th>MachineName
                                </th>
                                <th>ApplicationName
                                </th>
                                <th>IsCancellationRequested
                                </th>

                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td colSpan={4}>
                                    <b> { s.Executing.length } processes executing in { s.MachineName }</b>
                                </td>
                            </tr>
                            { s.Executing.map((item, i) =>
                                <tr key={i}>
                                    <td> <EntityLink lite={item.Process} inSearch={true} /> </td>
                                    <td> { item.State } </td>
                                    <td> { numbro(item.Progress).format("0 %") } </td>
                                    <td> { item.MachineName } </td>
                                    <td> { item.ApplicationName } </td>
                                    <td> { item.IsCancellationRequested } </td>
                                </tr>
                            ) }
                        </tbody>
                    </table>

                    <br />
                    <h2>Latest Processes</h2>
                     <SearchControl findOptions={{ 
                        queryName: ProcessEntity, 
                        orderOptions: [{ columnName: "CreationDate", orderType: "Descending" }], 
                        searchOnLoad: true, 
                        showFilters: false, 
                        pagination: { elementsPerPage: 10, mode: "Firsts"}}}/>
                </div>
            </div>
        );
    }
}



