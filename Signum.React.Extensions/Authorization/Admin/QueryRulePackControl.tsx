﻿import * as React from 'react'
import { Button } from 'react-bootstrap'
import * as numbro from 'numbro'
import { classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { notifySuccess }from '../../../../Framework/Signum.React/Scripts/Operations/EntityOperations'
import EntityLink from '../../../../Framework/Signum.React/Scripts/SearchControl/EntityLink'
import { TypeContext, ButtonsContext, IRenderButtons } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import { EntityLine, ValueLine } from '../../../../Framework/Signum.React/Scripts/Lines'

import { QueryDescription, SubTokensOptions } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName, PropertyRoute, getTypeInfos } from '../../../../Framework/Signum.React/Scripts/Reflection'
import { ModifiableEntity, EntityControlMessage, Entity, parseLite, getToString, JavascriptMessage } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { API } from '../AuthClient'
import { QueryRulePack, QueryAllowedRule, AuthAdminMessage, PermissionSymbol, AuthMessage, QueryAllowed } from '../Signum.Entities.Authorization'
import { ColorRadio, GrayCheckbox } from './ColoredRadios'

import "./AuthAdmin.css"

export default class QueryRulesPackControl extends React.Component<{ ctx: TypeContext<QueryRulePack> }> implements IRenderButtons {

    handleSaveClick = (bc: ButtonsContext) => {
        let pack = this.props.ctx.value;

        API.saveQueryRulePack(pack)
            .then(() => API.fetchQueryRulePack(pack.type.cleanName!, pack.role.id!))
            .then(newPack => {
                notifySuccess();
                bc.frame.onReload({ entity: newPack, canExecute: {} });
            })
            .done();
    }

    renderButtons(bc: ButtonsContext) {
        return [
            <Button bsStyle="primary" onClick={() => this.handleSaveClick(bc) }>{AuthMessage.Save.niceToString() }</Button>
        ];
    }


    render() {

        let ctx = this.props.ctx;

        return (
            <div>
                <div className="form-compact">
                    <EntityLine ctx={ctx.subCtx(f => f.role) }  />
                    <ValueLine ctx={ctx.subCtx(f => f.strategy) }  />
                    <EntityLine ctx={ctx.subCtx(f => f.type) }  />
                </div>
                <table className="table table-condensed sf-auth-rules">
                    <thead>
                        <tr>
                            <th>
                                { PermissionSymbol.niceName() }
                            </th>
                            <th style={{ textAlign: "center" }}>
                                {QueryAllowed.niceName("Allow")}
                            </th>
                            <th style={{ textAlign: "center" }}>
                                {QueryAllowed.niceName("EmbeddedOnly")}
                            </th>
                            <th style={{ textAlign: "center" }}>
                                {QueryAllowed.niceName("None")}
                            </th>
                            <th style={{ textAlign: "center" }}>
                                {AuthAdminMessage.Overriden.niceToString() }
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        { ctx.mlistItemCtxs(a => a.rules).orderBy(a => a.value.resource.key).map((c, i) =>
                            <tr key={i}>
                                <td>
                                    {c.value.resource.toStr}
                                </td>
                                <td style={{ textAlign: "center" }}>
                                    {this.renderRadio(c.value, "Allow", "green")}
                                </td>
                                <td style={{ textAlign: "center" }}>
                                    {this.renderRadio(c.value, "EmbeddedOnly", "#FFAD00")}
                                </td>
                                <td style={{ textAlign: "center" }}>
                                    {this.renderRadio(c.value, "None", "red")}
                                </td>
                                <td style={{ textAlign: "center" }}>
                                    <GrayCheckbox checked={c.value.allowed != c.value.allowedBase} onUnchecked={() => {
                                        c.value.allowed = c.value.allowedBase;
                                        ctx.value.modified = true; 
                                        this.forceUpdate();
                                    }} />
                                </td>
                            </tr>
                        )
                        }
                    </tbody>
                </table>

            </div>
        );
    }

    renderRadio(c: QueryAllowedRule, allowed: QueryAllowed, color: string) {

        if (c.coercedValues.contains(allowed))
            return;

        return <ColorRadio checked={c.allowed == allowed} color={color} onClicked={a => { c.allowed = allowed; c.modified = true; this.forceUpdate() } }/>;
    }
}



