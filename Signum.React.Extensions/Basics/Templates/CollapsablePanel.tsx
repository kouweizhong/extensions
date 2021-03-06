﻿import * as React from 'react'
import { classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import { BsStyle } from "../../../../Framework/Signum.React/Scripts/Operations";

export interface CollapsablePanelProps {
    type?: BsStyle;
    header?: React.ReactNode;
    body?: React.ReactNode;
    defaultOpen?: boolean;
    collapsable?: boolean;
}

export interface CollapsablePanelState {
    open: boolean,
    isRTL: boolean;
}

export default class CollapsablePanel extends React.Component<CollapsablePanelProps, CollapsablePanelState> {

    constructor(props: CollapsablePanelProps) {
        super(props);
        this.state = { open: this.props.defaultOpen == true, isRTL: document.body.classList.contains("rtl-mode") };
    }

    changeState = () => {
        this.setState({ open: !this.state.open });
    }

    render() {
        return (
            <div className={classes("panel", this.props.type ? "panel-" + this.props.type : "panel-default")}>
                <div className="panel-heading" onClick={this.changeState}>
                    {this.props.header}
                    {(this.props.collapsable == undefined || this.props.collapsable == true) &&
                        <span
                            className={classes(this.state.isRTL ? "pull-left" : "pull-right", "glyphicon", this.state.open ? "glyphicon-chevron-up" : "glyphicon-chevron-down")}
                            style={{ cursor: "pointer" }}
                            onClick={this.changeState}>
                        </span>}
                </div>
                {this.state.open &&
                    <div className="panel-body">
                        {this.props.body}
                    </div>}
            </div>
        );
    }
}